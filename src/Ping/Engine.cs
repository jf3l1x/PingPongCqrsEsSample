using System;
using System.Collections.Generic;
using System.Web.Http;
using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using LightInject;
using NEventStore;
using NEventStore.Persistence.Sql.SqlDialects;
using Owin;
using Ping.Configuration;
using Ping.EntityFramework;
using Ping.Handlers;
using Ping.Handlers.Commands;
using Ping.Handlers.Events;
using Ping.Model.Read;
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
            container.Register<EvtHandler>();
            container.RegisterInstance(_configuration.TenantConfigurator);
            container.Register<IRepository, EventStoreRepository>();
            container.Register<IConstructAggregates, AggregateFactory>();
            container.Register<IDetectConflicts, NullConflictDetection>();
            container.Register<IReadModelRepository<PingSummary>,PingSummaryContext>();
            if (_options.RunMode == RunMode.Sync)
            {
                container.Register<ICreateCmdHandle, SynchronousCmdHandlerFactory>();
                container.Register<ICreateEvtHandle, SynchronousEvtHandlerFactory>();
                container.Register<IServiceBus, SynchronousBus>();
                container.Register<IPipelineHook, SendToBus>();
                container.RegisterInstance(Wireup.Init()
                    .UsingSqlPersistence(new TenantConnectionFactory(_configuration.TenantConfigurator))
                    .WithDialect(new MsSqlDialect()).InitializeStorageEngine().UsingJsonSerialization()
                    .HookIntoPipelineUsing(container.GetInstance<IPipelineHook>())
                    .Build());
            }
            else
            {
                container.RegisterInstance(Wireup.Init()
                    .UsingSqlPersistence(new TenantConnectionFactory(_configuration.TenantConfigurator))
                    .WithDialect(new MsSqlDialect()).InitializeStorageEngine()
                    .UsingJsonSerialization().Build());
                container.Register<IServiceBus, SynchronousBus>();
            }
            
          
            return container;
        }
    }
}