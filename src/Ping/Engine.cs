using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using LightInject;
using NEventStore;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;
using Owin;
using Ping.Configuration;
using Ping.EntityFramework;
using Ping.Handlers;
using Ping.Handlers.Async;
using Ping.Handlers.Commands;
using Ping.Handlers.Sync;
using Ping.Model.Read;
using Ping.Services.Default;
using PingPong.Shared;
using Rebus;
using Rebus.Configuration;
using Rebus.RabbitMQ;

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
            container.Register<IReadModelRepository<PingSummary>, Dapper.Repository>();
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
                    .UsingSqlPersistence(container.GetInstance<IConnectionFactory>())
                    .WithDialect(new MsSqlDialect()).InitializeStorageEngine().UsingJsonSerialization()
                    .HookIntoPipelineUsing(container.GetInstance<IPipelineHook>())
                    .Build());
            }
            else
            {
                container.Register<IServiceBus, AsynchronousBus>();
                container.Register<ICreateHandlers, AsynchronousHandler>();
                container.RegisterInstance(Wireup.Init()
                    .UsingSqlPersistence(container.GetInstance<IConnectionFactory>())
                    .WithDialect(new MsSqlDialect()).InitializeStorageEngine()
                    .UsingJsonSerialization()
                    .HookIntoPipelineUsing(container.GetInstance<IPipelineHook>()).Build());
            }


            return container;
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