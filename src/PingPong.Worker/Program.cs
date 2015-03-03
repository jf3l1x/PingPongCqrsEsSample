using System;
using System.Collections.Generic;
using System.Linq;
using LightInject;
using Ping.Configuration;
using PingPong.Shared;
using Pong;

namespace PingPong.Worker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = CreateInjectorContainer();
            var engines = container.GetAllInstances<IModuleEngine>().ToList();
            engines.ForEach(e => e.StartListener());
            Console.WriteLine("Press some key to end!");
            Console.Read();
        }

        private static ServiceContainer CreateInjectorContainer()
        {
            var container = new ServiceContainer();
            container.RegisterInstance(Configure());
            container.Register<IModuleEngine,Ping.Engine>("ping");
            container.Register<IModuleEngine, Pong.Engine>("pong");
            container.RegisterInstance(new PingOptions(){RunMode = RunMode.Sync});
            
            return container;
        }

        private static IModuleConfiguration Configure()
        {
            var tenantConfigurator = new TenantConfigurator("Server=.;Database=pingpong;Trusted_Connection=True;");
            
            
            return new MemoryConfiguration(tenantConfigurator, "amqp://jf3l1x:password@localhost:5672/testes"){ReceiveMessages = true};
        }
    }
}