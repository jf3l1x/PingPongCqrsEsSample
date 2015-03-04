﻿using System;
using System.Globalization;
using System.Linq;
using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using LightInject;
using NEventStore;
using NEventStore.Persistence.Sql.SqlDialects;
using Owin;
using PingPong.Shared;
using Pong.EntityFramework;
using Pong.Handlers.Async;
using Pong.Handlers.Commands;
using Pong.Messages.Commands;
using Pong.Model.Read;
using Pong.Services.Default;
using Rebus;
using Rebus.Configuration;
using Rebus.RabbitMQ;

namespace Pong
{
    public  class Engine : IModuleEngine
    {
        private readonly IModuleConfiguration _configuration;

        public Engine(IModuleConfiguration configuration)
        {
            _configuration = configuration;
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
            container.Register<IRepository, EventStoreRepository>();
            container.Register<IReadModelRepository<PongSummary>, PongSummaryContext>();
            container.Register<IConstructAggregates, AggregateFactory>();
            container.Register<IDetectConflicts, NullConflictDetection>();
            container.Register<IDetermineMessageOwnership, MessageRouter>();
            container.Register<IContainerAdapter, MyContainerAdapter>();
            container.Register <IResolveTypes,DefaultTypeResolver>();
            container.Register<IMutateMessages, DefaultMessageMutator>();
            container.Register<RebusHandler>();

            container.RegisterInstance(
                Configure.With(container.GetInstance<IContainerAdapter>())

                    .Transport(t => t.UseRabbitMq(_configuration.BusConnectionString, "pong", "pongErrors").ManageSubscriptions().UseExchange("Rebus").AddEventNameResolver(type => "ESB")).Events(r => r.MessageMutators.Add(container.GetInstance<IMutateMessages>()))
                    .MessageOwnership(d => d.Use(container.GetInstance<IDetermineMessageOwnership>()))
                    .CreateBus().Start());

            container.Register<IPipelineHook, SendToBus>();

            container.Register<IServiceBus, AsynchronousBus>();
            container.Register<ICreateHandlers, AsynchronousHandler>();
            container.RegisterInstance(Wireup.Init()
                .UsingSqlPersistence(new TenantConnectionFactory(_configuration.TenantConfigurator))
                .WithDialect(new MsSqlDialect()).InitializeStorageEngine()
                .UsingJsonSerialization()
                .HookIntoPipelineUsing(container.GetInstance<IPipelineHook>()).Build());
        }
    }

    internal class DefaultTypeResolver:IResolveTypes
    {
        public Type ResolveType(string eventName)
        {
            return
                typeof (Engine).Assembly.GetTypes()
                    .FirstOrDefault(
                        t =>
                            String.Compare(t.Name, eventName, StringComparison.InvariantCultureIgnoreCase)==0);
        }
    }
}