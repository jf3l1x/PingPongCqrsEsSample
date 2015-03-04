﻿using System;
using System.Data;
using System.Linq;
using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using LightInject;
using NEventStore;
using NEventStore.Persistence.Sql;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using Owin;
using Ping.Extensions;
using PingPong.Shared;
using Pong.Configuration;
using Pong.Handlers.Async;
using Pong.Handlers.Commands;
using Pong.Model.Read;
using Pong.Persistence.Dapper;
using Pong.Persistence.EntityFramework;
using Pong.Persistence.NHibernate;
using Pong.Persistence.NHibernate.Mappings;
using Pong.Services.Default;
using Rebus;
using Rebus.Configuration;
using Rebus.RabbitMQ;

namespace Pong
{
    public class Engine : IModuleEngine
    {
        private readonly IModuleConfiguration _configuration;
        private readonly PongOptions _options;

        public Engine(IModuleConfiguration configuration, PongOptions options)
        {
            _configuration = configuration;
            _options = options;
        }

        public IAppBuilder RegisterApi(IAppBuilder config)
        {
            return config;
        }

        public void StartListener()
        {
            CreateContainer();
        }

        private void CreateContainer()
        {
            var container = new ServiceContainer();

            container.RegisterInstance(_configuration.TenantConfigurator);

            container.Register<DefaultHandler>();
            container.Register<IConnectionFactory, TenantConnectionFactory>();
            container.Register<IRepository, EventStoreRepositoryWithSnapshot>();

            ConfigureReadPersistenceModel(container);

            container.Register<IConstructAggregates, AggregateFactory>();
            container.Register<IDetectConflicts, NullConflictDetection>();
            container.Register<IDetermineMessageOwnership, MessageRouter>();
            container.Register<IContainerAdapter, MyContainerAdapter>();
            container.Register<IResolveTypes, DefaultTypeResolver>();
            container.Register<IMutateMessages, DefaultMessageMutator>();
            container.Register<RebusHandler>();

            container.RegisterInstance(
                Configure.With(container.GetInstance<IContainerAdapter>())
                    .Transport(
                        t =>
                            t.UseRabbitMq(_configuration.BusConnectionString, "pong", "pongErrors")
                                .ManageSubscriptions()
                                .UseExchange("Rebus")
                                .AddEventNameResolver(type => "ESB"))
                    .Events(r => r.MessageMutators.Add(container.GetInstance<IMutateMessages>()))
                    .MessageOwnership(d => d.Use(container.GetInstance<IDetermineMessageOwnership>()))
                    .CreateBus().Start());

            container.Register<IPipelineHook, SendToBus>("notifier");
            

            container.Register<IServiceBus, AsynchronousBus>();
            container.Register<ICreateHandlers, AsynchronousHandler>();
            container.RegisterInstance(Wireup.Init()
                .ConfigurePersistence(_options, container)
                .UsingJsonSerialization()
                .HookIntoPipelineUsing(container.GetAllInstances<IPipelineHook>()).Build());
        }

        private void ConfigureReadPersistenceModel(ServiceContainer container)
        {
            switch (_options.ReadModelPersistenceMode)
            {
                case PersistenceMode.NHibernate:
                    container.Register(factory => CreateNHibernateSessionFactory(factory),
                        new PerContainerLifetime());
                    container.Register(
                        factory =>
                            factory.GetInstance<ISessionFactory>()
                                .OpenStatelessSession());
                    container.Register<IReadModelRepository<PongSummary>, PongSummaryRepository>();
                    break;
                case PersistenceMode.Dapper:
                    container.Register<IReadModelRepository<PongSummary>, Repository>();
                    break;
                case PersistenceMode.PetaPoco:
                    container.Register<IReadModelRepository<PongSummary>, Persistence.PetaPoco.Repository>();
                    break;
                default:
                    // Default is Entity Framework.
                    container.Register<IReadModelRepository<PongSummary>, PongSummaryContext>();
                    break;
            }
        }

        private ISessionFactory CreateNHibernateSessionFactory(IServiceFactory factory)
        {
            var mapper = new ModelMapper();
            var configuration = new NHibernate.Cfg.Configuration();

            mapper.AddMapping<PongSummaryMap>();

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