using LightInject;
using Owin;
﻿using Ping.Configuration;
﻿using PingPong.Shared;
using Pong.Configuration;
using PersistenceMode = Ping.Configuration.PersistenceMode;

namespace PingPong.WebHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = CreateInjectorContainer();

            app.Map("/ping", map => container.GetInstance<IModuleEngine>("ping").RegisterApi(map));
            app.Map("/pong", map => container.GetInstance<IModuleEngine>("pong").RegisterApi(map));
        }

        private static ServiceContainer CreateInjectorContainer()
        {
            var container = new ServiceContainer();
            container.RegisterInstance(Configure());
            container.RegisterInstance(new PingOptions
            {
                RunMode = RunMode.Async,
                ReadModelPersistenceMode = PersistenceMode.NHibernate
            });

            container.RegisterInstance(new PongOptions
            {
                
            });

            container.Register<IModuleEngine, Ping.Engine>("ping");
            container.Register<IModuleEngine, Pong.Engine>("pong");
            

            return container;
        }

       
        private static IModuleConfiguration Configure()
        {
            var tenantConfigurator = new TenantConfigurator("Server=.;Database=pingpong;Trusted_Connection=True;");
            

            return new MemoryConfiguration(tenantConfigurator, "amqp://jf3l1x:password@localhost:5672/testes");
        }
    }
}
