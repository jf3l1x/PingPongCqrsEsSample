using System.Data;
using Constant.Module.Interfaces;
using Constant.Module.Interfaces.Configuration;
using LightInject;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using Owin;
using Ping.Persistence.Nhibernate;
using Ping.Web;
using PingPong.Shared;
using Pong.Configuration;
using Serilog;

namespace PingPong.WebHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ServiceContainer container = CreateInjectorContainer();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole().MinimumLevel.Error()
                .CreateLogger();
            app.Map("/ping",
                map => container.GetInstance<IWebModule>("ping").RegisterApi(map, new WebModuleContainer(container)));
            app.Map("/pong", map => container.GetInstance<IModuleEngine>("pong").RegisterApi(map));
        }

        private static ServiceContainer CreateInjectorContainer()
        {
            var container = new ServiceContainer();
            container.Register<IGiveTenantConfiguration, TenantConfigurator>();
            container.Register<IModuleConfiguration, MemoryConfiguration>();
            ConfigureReadModelPersistence(container);
            container.RegisterInstance(new PongOptions());

            container.Register<IWebModule, Engine>("ping");
            container.Register<IModuleEngine, Pong.Engine>("pong");


            return container;
        }

        private static void ConfigureReadModelPersistence(ServiceContainer container)
        {
            container.RegisterInstance(CreateNHibernateSessionFactory(container));
            container.Register(
                ctx =>
                    ctx.GetInstance<ISessionFactory>()
                        .OpenStatelessSession());
        }

        private static ISessionFactory CreateNHibernateSessionFactory(ServiceContainer container)
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