using Constant.Module.Interfaces;
using LightInject;
using Owin;
using Ping.Web.Configuration;
using PingPong.Shared;
using Pong.Configuration;

namespace PingPong.WebHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = CreateInjectorContainer();

            app.Map("/ping", map => container.GetInstance<IWebModule>("ping").RegisterApi(map,new WebModuleContainer()));
            app.Map("/pong", map => container.GetInstance<IModuleEngine>("pong").RegisterApi(map));
        }

        private static ServiceContainer CreateInjectorContainer()
        {
            var container = new ServiceContainer();
            container.RegisterInstance(Configure());
           
            container.RegisterInstance(new PongOptions
            {
                
            });

            container.Register<IWebModule, Ping.Web.Engine>("ping");
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
