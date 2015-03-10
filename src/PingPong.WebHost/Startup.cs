using Constant.Module.Interfaces;
using Constant.Module.Interfaces.Configuration;
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
            container.Register<IGiveTenantConfiguration,TenantConfigurator>();
            container.Register<IModuleConfiguration,MemoryConfiguration>();
           
            container.RegisterInstance(new PongOptions
            {
                
            });

            container.Register<IWebModule, Ping.Web.Engine>("ping");
            container.Register<IModuleEngine, Pong.Engine>("pong");
            

            return container;
        }

       
       
    }
}
