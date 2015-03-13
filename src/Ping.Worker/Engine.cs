using System;
using System.Data;
using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using Constant.Hosting.Rebus;
using Constant.Module.Interfaces;
using Constant.Module.Interfaces.Bus;
using Constant.Module.Interfaces.Configuration;
using LightInject;
using NEventStore;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using Ping.Persistence.Nhibernate;
using Ping.Shared.Messages.ExternalEvents;
using Ping.Shared.Services;
using Ping.Worker.Handlers;
using Ping.Worker.Services;
using Rebus.RabbitMQ;
using Serilog;

namespace Ping.Worker
{
    public class Engine : MarshalByRefObject, IWorkerModule
    {
        public void Start(IWorkerModuleContainer wmc)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole().MinimumLevel.Error()
                .CreateLogger();
            ServiceContainer container = CreateContainer(wmc);
            var bus = container.GetInstance<IServiceBus>();
            bus.Start();
            bus.Subscribe<PongSent>();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }


        private ServiceContainer CreateContainer(IWorkerModuleContainer factory)
        {
            var container = new ServiceContainer();

            container.Register<DefaultHandler>();

            container.Register(ctx=>factory.CreateTenantConfiguration());
            container.Register<IRepository, EventStoreRepository>();
            container.Register<IConstructAggregates, AggregateFactory>();
            container.Register<IDetectConflicts, NullConflictDetection>();
            ConfigureReadModelPersistence(container);
            container.Register<DefaultHandler>();
            container.Register<IActivateHandlers, HandlerActivator>();
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
                .Build());

            return container;
        }

        private void ConfigureReadModelPersistence(ServiceContainer container)
        {
            container.RegisterInstance(CreateNHibernateSessionFactory(container));
            container.Register(
                ctx =>
                    ctx.GetInstance<ISessionFactory>()
                        .OpenStatelessSession());
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
                foreach (IResolveTypeName resolver in container.GetAllInstances<IResolveTypeName>())
                {
                    IResolveTypeName r = resolver;
                    options.AddEventNameResolver(r.Resolve);
                }
            });

            container.RegisterInstance<IServiceBus>(bus);
        }

        private ISessionFactory CreateNHibernateSessionFactory(ServiceContainer container)
        {
            var configuration = new Configuration();
            configuration.DataBaseIntegration(x =>
            {
                x.ConnectionString = container.GetInstance<IGiveTenantConfiguration>().GetReadModelConnectionString();
                x.IsolationLevel = IsolationLevel.ReadUncommitted;
                x.Driver<Sql2008ClientDriver>();
                x.Dialect<MsSql2012Dialect>();
                x.BatchSize = 50;
                x.Timeout = 30;
            });
            configuration.ConfigurePingPersistence(new ContainerAdapter(container));
            ISessionFactory sessionFactory = configuration.BuildSessionFactory();
            return sessionFactory;
        }
    }
}