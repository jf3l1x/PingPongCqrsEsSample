using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using LightInject;
using LightInject.Interception;
using MongoDB.Driver;
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
using Ping.Messages.ExternalEvents;
using Ping.Model.Read;
using Ping.Persistence.Dapper;
using Ping.Persistence.NHibernate;
using Ping.Persistence.NHibernate.Mappings;
using Ping.Services.Default;
using PingPong.Shared;
using PingPong.Shared.LightInject;
using Rebus;
using Rebus.Configuration;
using Rebus.RabbitMQ;
using DefaultNamingStrategy = NEventStore.Persistence.EventStore.Services.Naming.DefaultNamingStrategy;
using NHibernateConfiguration = NHibernate.Cfg.Configuration;
using PingSummaryRepository = Ping.Persistence.EntityFramework.PingSummaryRepository;

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
            HttpConfiguration configuration = CreateHttpConfiguration();
            ServiceContainer container = CreateContainer();
            container.RegisterApiControllers();
            container.EnableWebApi(configuration);
            app.UseWebApi(configuration);


            return app;
        }

        public void StartListener()
        {
            var container=CreateContainer();
            container.GetInstance<IBus>().Subscribe<PongSent>();
        }

        private HttpConfiguration CreateHttpConfiguration()
        {
            var configuration = new HttpConfiguration();
            configuration.MapHttpAttributeRoutes();
            
            // OData
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<PingSummary>("PingSummaries");

            configuration.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());

            return configuration;
        }

        private ServiceContainer CreateContainer()
        {
            var container = new ServiceContainer();

            container.Register<DefaultHandler>();

            container.RegisterInstance(_configuration.TenantConfigurator);
            container.Register<IRepository, EventStoreRepositoryWithSnapshot>();
            container.Register<IConstructAggregates, AggregateFactory>();
            container.Register<IDetectConflicts, NullConflictDetection>();

            ConfigureReadPersistenceModel(container);
            
            container.Register<IDetermineMessageOwnership, MessageRouter>();
            container.Register<IContainerAdapter, MyContainerAdapter>();
            container.Register<IResolveTypes, DefaultTypeResolver>();
            container.Register<IMutateMessages, DefaultMessageMutator>();
            
            container.Register<IConnectionFactory,TenantConnectionFactory>();

            container.Register<RebusHandler>();

            ConfigureInterceptors(container);

            if (_configuration.ReceiveMessages)
            {
                container.RegisterInstance(
                    Rebus.Configuration.Configure.With(container.GetInstance<IContainerAdapter>()).Events(r => r.MessageMutators.Add(container.GetInstance<IMutateMessages>()))
                    .Transport(t => t.UseRabbitMq(_configuration.BusConnectionString, "ping", "pingErrors").ManageSubscriptions().UseExchange("Rebus").AddEventNameResolver(ResolveTypeName))
                        .MessageOwnership(d => d.Use(container.GetInstance<IDetermineMessageOwnership>()))
                        .CreateBus().Start());
            }
            else
            {
                container.RegisterInstance(
                    Rebus.Configuration.Configure.With(container.GetInstance<IContainerAdapter>()).Events(r => r.MessageMutators.Add(container.GetInstance<IMutateMessages>()))
                        .Transport(t => t.UseRabbitMqInOneWayMode(_configuration.BusConnectionString).ManageSubscriptions().UseExchange("Rebus").AddEventNameResolver(ResolveTypeName))
                        .MessageOwnership(d => d.Use(container.GetInstance<IDetermineMessageOwnership>()))
                        .CreateBus().Start());
            }
            
            container.Register<IPipelineHook, SendToBus>();

            if (_options.RunMode == RunMode.Sync)
            {
                container.Register<ICreateHandlers, SynchronousCmdHandlerFactory>();
                container.Register<IServiceBus, SynchronousBus>();
               
            }
            else
            {
                container.Register<IServiceBus, AsynchronousBus>();
                container.Register<ICreateHandlers, AsynchronousHandler>();
               
            }
            container.RegisterInstance(Wireup.Init()
                   .ConfigurePersistence(_options, container)
                   .UsingJsonSerialization()
                   
                   .HookIntoPipelineUsing(container.GetAllInstances<IPipelineHook>())
                   .Build());

            return container;
        }

        private string ResolveTypeName(Type type)
        {
            if (type == typeof (PongRequested))
                return "pongrequested";
            if (type == typeof (PongSent))
                return "pongsent";
            return string.Empty;
        }

        private void ConfigureInterceptors(ServiceContainer container)
        {
            // All Handle methods of RebusHandler class are Begin Scope.
            container.Intercept(x => x.ServiceType == typeof(RebusHandler), (sf, pd) => pd.Implement(() => new BeginScopeInterceptor(container), m => m.Name == "Handle"));
        }

        private void ConfigureReadPersistenceModel(ServiceContainer container)
        {
            switch (_options.ReadModelPersistenceMode)
            {
                case ReadPersistenceMode.NHibernate:
                    container.Register(factory => CreateNHibernateSessionFactory(factory), new PerContainerLifetime());
                    container.Register(factory => factory.GetInstance<ISessionFactory>().OpenStatelessSession(), new PerScopeLifetime());
                    container.Register<IReadModelRepository<PingSummary>, Persistence.NHibernate.PingSummaryRepository>();
                    break;
                case ReadPersistenceMode.Dapper:
                    container.Register<IReadModelRepository<PingSummary>, Persistence.Dapper.Repository>();
                    break;
                case ReadPersistenceMode.PetaPoco:
                    container.Register<IReadModelRepository<PingSummary>, Persistence.PetaPoco.Repository>();
                    break;
                case ReadPersistenceMode.MongoDB:
                    //http://docs.mongodb.org/ecosystem/tutorial/use-csharp-driver/
                    container.Register(factory => new MongoClient(_configuration.TenantConfigurator.GetReadModelConnectionString()), new PerContainerLifetime()); // Connect to localhost
                    container.Register(factory =>  container.GetInstance<MongoClient>().GetServer(), new PerContainerLifetime());
                    container.Register(factory => container.GetInstance<MongoServer>().GetDatabase("testes"));
                    container.Register<IReadModelRepository<PingSummary>, Persistence.MongoDB.PingSummaryRepositoryMongoDB>();
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
                x.ConnectionString = _configuration.TenantConfigurator.GetReadModelConnectionString();
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