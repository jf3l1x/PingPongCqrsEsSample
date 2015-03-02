using System;
using System.Web.Http;
using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using LightInject;
using NEventStore;
using NEventStore.Persistence.Sql.SqlDialects;
using Owin;
using Ping.Configuration;
using Ping.Handlers;
using Ping.Handlers.Commands;
using Ping.Services.Default;
using PingPong.Shared;

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
            Console.WriteLine(_configuration);
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
            
            container.Register<CmdHandler>();
            if (_options.RunMode == RunMode.Sync)
            {
                container.Register<ICreateHandle, SynchronousHandlerFactory>();
            }
            container.Register<IServiceBus,VoidBus>();
            container.Register<IRepository,EventStoreRepository>();
            container.Register<IConstructAggregates,AggregateFactory>();
            container.Register<IDetectConflicts, NullConflictDetection>();
            container.RegisterInstance(Wireup.Init()
                .UsingSqlPersistence(new TenantConnectionFactory(_configuration.TenantConfigurator))
                .WithDialect(new MsSqlDialect()).InitializeStorageEngine()
                .UsingJsonSerialization().Build());
            return container;
        }
    }
}