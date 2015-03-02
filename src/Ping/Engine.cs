using System;
using System.Web.Http;
using LightInject;
using Owin;
using Ping.Configuration;
using Ping.Handlers;
using Ping.Handlers.Commands;
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
            
            return container;
        }
    }
}