using System;
using System.Linq;
using CommonDomain;
using CommonDomain.Persistence;
using Constant.Module.Interfaces;
using Constant.Module.Interfaces.Configuration;
using LightInject;
using NEventStore;
using NEventStore.Persistence.Sql;
using Ping.Shared.Messages.ExternalEvents;
using Ping.Shared.Model.Read;
using Ping.Worker.Configuration;
using Ping.Worker.Extensions;
using Ping.Worker.Handlers.Async;
using Ping.Worker.Handlers.Commands;
using Ping.Worker.Handlers.Sync;
using Ping.Worker.Persistence.Dapper;
using Ping.Worker.Persistence.EntityFramework;
using Ping.Worker.Services.Default;
using PingPong.Shared;
using Rebus;
using Rebus.Configuration;
using Rebus.RabbitMQ;

namespace Ping.Worker
{
    public class Engine : IWorkerModule
    {
        private readonly IModuleConfiguration _configuration;
        private readonly PingOptions _options;

        public Engine(IModuleConfiguration configuration, PingOptions options)
        {
            _configuration = configuration;
            _options = options;
        }

        public void Start(IWorkerModuleContainer container)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
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
            container.Register<IResolveTypes, DefaultTypeResolver>();
            container.Register<IMutateMessages, DefaultMessageMutator>();

            container.Register<IConnectionFactory, TenantConnectionFactory>();
            container.Register<RebusHandler>();

            if (_configuration.ReceiveMessages)
            {
                IBus b;

                container.RegisterInstance(
                    Configure
                        .With(
                            container.GetInstance<IContainerAdapter>())
                        .Events(r => r.MessageMutators.Add(container.GetInstance<IMutateMessages>()))
                        .Transport(
                            t =>
                                t.UseRabbitMq(_configuration.BusConnectionString, "ping", "pingErrors")
                                    .ManageSubscriptions()
                                    .UseExchange("Rebus")
                                    .AddEventNameResolver(ResolveTypeName))
                        .MessageOwnership(d => d.Use(container.GetInstance<IDetermineMessageOwnership>()))
                        .CreateBus().Start());
            }
            else
            {
                container.RegisterInstance(
                    Configure.With(container.GetInstance<IContainerAdapter>())
                        .Events(r => r.MessageMutators.Add(container.GetInstance<IMutateMessages>()))
                        .Transport(
                            t =>
                                t.UseRabbitMqInOneWayMode(_configuration.BusConnectionString)
                                    .ManageSubscriptions()
                                    .UseExchange("Rebus")
                                    .AddEventNameResolver(ResolveTypeName))
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

        private void ConfigureReadPersistenceModel(ServiceContainer container)
        {
            switch (_options.ReadModelPersistenceMode)
            {
                case ReadPersistenceMode.Dapper:
                    container.Register<IReadModelRepository<PingSummary>, Repository>();
                    break;
                case ReadPersistenceMode.PetaPoco:
                    container.Register<IReadModelRepository<PingSummary>, Persistence.PetaPoco.Repository>();
                    break;
                default:
                    // Default is Entity Framework.
                    container.Register<IReadModelRepository<PingSummary>, PingSummaryRepository>();
                    break;
            }
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