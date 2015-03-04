using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using LightInject;
using NEventStore;
using NEventStore.Persistence.EventStore;
using NEventStore.Persistence.EventStore.Services;
using NEventStore.Persistence.EventStore.Services.Naming;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using Owin;
using Ping.Configuration;
using Ping.Extensions;
using Ping.Handlers;
using Ping.Handlers.Async;
using Ping.Handlers.Commands;
using Ping.Handlers.Sync;
using Ping.Model.Read;
using Ping.Persistence.Dapper;
using Ping.Persistence.EntityFramework;
using Ping.Persistence.NHibernate.Mappings;
using Ping.Services.Default;
using PingPong.Shared;
using Rebus;
using Rebus.Configuration;
using Rebus.RabbitMQ;
using DefaultNamingStrategy = NEventStore.Persistence.EventStore.Services.Naming.DefaultNamingStrategy;
using NHibernateConfiguration = NHibernate.Cfg.Configuration;

namespace Ping
{
    public class Engine : IModuleEngine
    {
        private readonly IModuleConfiguration _configuration;
        private readonly PingOptions _options;

        public Engine(IModuleConfiguration configuration, PingOptions options)
        {
            _configuration = configuration;
            _options = options;
        }

        public IAppBuilder RegisterApi(IAppBuilder app)
        {
            HttpConfiguration configuration = Configure();
            ServiceContainer container = CreateContainer();
            container.RegisterApiControllers();
            container.EnableWebApi(configuration);

            app.UseWebApi(configuration);
            return app;
        }

        public void StartListener()
        {
            CreateContainer();
            
        }

        private HttpConfiguration Configure()
        {
            var configuration = new HttpConfiguration();
            configuration.MapHttpAttributeRoutes();

            return configuration;
        }

        private ServiceContainer CreateContainer()
        {
            var container = new ServiceContainer();

            container.Register<DefaultHandler>();

            container.RegisterInstance(_configuration.TenantConfigurator);
            container.Register<IRepository, EventStoreRepository>();
            container.Register<IConstructAggregates, AggregateFactory>();
            container.Register<IDetectConflicts, NullConflictDetection>();

            ConfigureReadPersistenceModel(container);
            
            container.Register<IDetermineMessageOwnership, MessageRouter>();
            container.Register<IContainerAdapter, MyContainerAdapter>();
            container.Register<IResolveTypes, DefaultTypeResolver>();
            container.Register<IMutateMessages, DefaultMessageMutator>();
            
            container.Register<IConnectionFactory,TenantConnectionFactory>();
            container.Register<RebusHandler>();

            if (_configuration.ReceiveMessages)
            {
                container.RegisterInstance(
                    Rebus.Configuration.Configure.With(container.GetInstance<IContainerAdapter>()).Events(r => r.MessageMutators.Add(container.GetInstance<IMutateMessages>()))
                    .Transport(t => t.UseRabbitMq(_configuration.BusConnectionString, "ping", "pingErrors").ManageSubscriptions().UseExchange("Rebus").AddEventNameResolver(type=>"ESB"))
                        .MessageOwnership(d => d.Use(container.GetInstance<IDetermineMessageOwnership>()))
                        .CreateBus().Start());
            }
            else
            {
                container.RegisterInstance(
                    Rebus.Configuration.Configure.With(container.GetInstance<IContainerAdapter>()).Events(r => r.MessageMutators.Add(container.GetInstance<IMutateMessages>()))
                        .Transport(t => t.UseRabbitMqInOneWayMode(_configuration.BusConnectionString).ManageSubscriptions().UseExchange("Rebus").AddEventNameResolver(type => "ESB"))
                        .MessageOwnership(d => d.Use(container.GetInstance<IDetermineMessageOwnership>()))
                        .CreateBus().Start());
            }
            
            container.Register<IPipelineHook, SendToBus>();

            if (_options.RunMode == RunMode.Sync)
            {
                container.Register<ICreateHandlers, SynchronousCmdHandlerFactory>();
                container.Register<IServiceBus, SynchronousBus>();
                container.RegisterInstance(Wireup.Init()
                    .ConfigurePersistence(_options, container)
                    .UsingJsonSerialization()
                    .HookIntoPipelineUsing(container.GetInstance<IPipelineHook>())
                    .Build());
            }
            else
            {
                container.Register<IServiceBus, AsynchronousBus>();
                container.Register<ICreateHandlers, AsynchronousHandler>();
                container.RegisterInstance(Wireup.Init()
                    .ConfigurePersistence(_options,container)
                    .UsingJsonSerialization()
                    .HookIntoPipelineUsing(container.GetInstance<IPipelineHook>()).Build());
            }


            return container;
        }

        private void ConfigureReadPersistenceModel(ServiceContainer container)
        {
            switch (_options.ReadModelPersistenceMode)
            {
                case PersistenceMode.NHibernate:
                    container.Register<ISessionFactory>(factory => CreateNHibernateSessionFactory(factory),
                        new PerContainerLifetime());
                    container.Register<IStatelessSession>(
                        factory =>
                            factory.GetInstance<ISessionFactory>().OpenStatelessSession());
                    container.Register<IReadModelRepository<PingSummary>, Persistence.NHibernate.PingSummaryRepository>();
                    break;
                case PersistenceMode.Dapper:
                    container.Register<IReadModelRepository<PingSummary>, Persistence.Dapper.Repository>();
                    break;
                case PersistenceMode.PetaPoco:
                    container.Register<IReadModelRepository<PingSummary>, Persistence.PetaPoco.Repository>();
                    break;
                default:
                    // Default is Entity Framework.
                    container.Register<IReadModelRepository<PingSummary>, PingSummaryRepository>();
                    break;
            }
        }
        
        private ISessionFactory CreateNHibernateSessionFactory(IServiceFactory factory)
        {
            var mapper = new ModelMapper();
            var configuration = new NHibernateConfiguration();

            mapper.AddMapping<PingSummaryMap>();

            configuration.DataBaseIntegration(x =>
            {
                x.ConnectionString = _configuration.TenantConfigurator.GetConnectionString();
                x.IsolationLevel = IsolationLevel.ReadUncommitted;
                x.Driver<Sql2008ClientDriver>();
                x.Dialect<MsSql2012Dialect>();
                x.BatchSize = 50;
                x.Timeout = 30;

#if DEBUG
                x.LogSqlInConsole = true;
                x.LogFormattedSql = true;
                x.AutoCommentSql = true;
                
#endif
            });

            
#if DEBUG
            configuration.SessionFactory().GenerateStatistics();
#endif

              configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

            ISessionFactory sessionFactory = configuration.BuildSessionFactory();

            return sessionFactory;
        }
    }
    internal class DefaultTypeResolver : IResolveTypes
    {
        public Type ResolveType(string eventName)
        {
            return
                typeof (Engine).Assembly.GetTypes()
                    .FirstOrDefault(
                        t =>
                            String.Compare(t.Name, eventName, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
    }
}