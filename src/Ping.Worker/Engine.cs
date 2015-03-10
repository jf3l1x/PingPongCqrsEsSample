using System;
using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using Constant.Hosting.Rebus;
using Constant.Module.Interfaces;
using Constant.Module.Interfaces.Bus;
using Constant.Module.Interfaces.Configuration;
using Constant.Module.Interfaces.Persistence.ReadModel;
using LightInject;
using NEventStore;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;
using Ping.Shared.Messages.ExternalEvents;
using Ping.Shared.Model.Read;
using Ping.Shared.Services;
using Ping.Worker.Handlers;
using Ping.Worker.Persistence.Dapper;
using Ping.Worker.Services;
using Rebus.RabbitMQ;

namespace Ping.Worker
{
    public class Engine : MarshalByRefObject, IWorkerModule
    {
        public void Start(IWorkerModuleContainer wmc)
        {
            var container = CreateContainer(wmc);
            var bus = container.GetInstance<IServiceBus>();
            bus.Start();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }


        private ServiceContainer CreateContainer(IWorkerModuleContainer factory)
        {
            var container = new ServiceContainer();

            container.Register<DefaultHandler>();

            container.RegisterInstance(factory.CreateTenantConfiguration());
            container.Register<IRepository, EventStoreRepository>();
            container.Register<IConstructAggregates, AggregateFactory>();
            container.Register<IDetectConflicts, NullConflictDetection>();

            ConfigureReadPersistenceModel(container);
            container.Register<DefaultHandler>();
            container.Register<IActivateHandlers,HandlerActivator>();
            container.Register<IRouteMessages, MessageRouter>();
            container.Register<IResolveTypeName, TypeNameResolver>();
            container.Register<IMutateMessages, DefaultMessageMutator>();
            container.Register<IConnectionFactory, SqlConnectionFactory>();


            RegisterRebus(container);


            container.Register<IPipelineHook, InvokeHandler>();


            container.RegisterInstance(Wireup.Init()
                .UsingSqlPersistence(container.GetInstance<IConnectionFactory>())
                .WithDialect(new MsSqlDialect())
                .UsingJsonSerialization()
                .HookIntoPipelineUsing(container.GetAllInstances<IPipelineHook>())
                .Build());

            return container;
        }

        private void RegisterRebus(ServiceContainer container)
        {
            var bus = new RebusAdapter();
            bus.RegisterActivatorFactory(container.GetInstance<IActivateHandlers>);
            bus.RegisterMutators(container.GetAllInstances<IMutateMessages>());
            bus.SetMessageRouter(container.GetInstance<IRouteMessages>());
            bus.GetConfigurator().Transport(transport =>
            {
                RabbitMqOptions options = transport.UseRabbitMq(
                    container.GetInstance<IGiveTenantConfiguration>().GetBusConnectionString(), "ping", "pingErrors")
                    .ManageSubscriptions()
                    .UseExchange("Rebus");
                foreach (var resolver in container.GetAllInstances<IResolveTypeName>())
                {
                    var r = resolver;
                    options.AddEventNameResolver(r.Resolve);
                }
            });
            
            container.RegisterInstance<IServiceBus>(bus);
        }

        private void ConfigureReadPersistenceModel(ServiceContainer container)
        {
            container.Register<IReadRepository<PingSummary>, Repository>();
        }
    }
}