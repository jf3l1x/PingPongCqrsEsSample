using System;
using Constant.Module.Interfaces;
using LightInject;
using PingPong.Shared;
using Pong;
using Pong.Configuration;

namespace PingPong.Worker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ServiceContainer container = CreateInjectorContainer();
            container.GetInstance<IModuleEngine>().StartListener();
            //container.GetInstance<IWorkerModule>().Start(null);

            Console.WriteLine("Press some key to end!");
            Console.Read();
        }

        private static ServiceContainer CreateInjectorContainer()
        {
            var container = new ServiceContainer();
            container.RegisterInstance(Configure());
            container.Register<IWorkerModule, Ping.Worker.Engine>("ping");
            container.Register<IModuleEngine, Engine>("pong");

            //container.RegisterInstance(new PingOptions
            //{
            //    RunMode = RunMode.Sync,
            //    ReadModelPersistenceMode = ReadPersistenceMode.EntityFramework,
            //    WriteModelPersistenceMode = WritePersistenceMode.SqlServer
            //});

            container.RegisterInstance(new PongOptions
            {
                ReadModelPersistenceMode = ReadPersistenceMode.EntityFramework,
                WriteModelPersistenceMode = WritePersistenceMode.SqlServer
            });

            return container;
        }

        private static IModuleConfiguration Configure()
        {
            var tenantConfigurator = new TenantConfigurator("Server=.;Database=pingpong;Trusted_Connection=True;");


            return new MemoryConfiguration(tenantConfigurator, "amqp://jf3l1x:password@localhost:5672/testes")
            {
                ReceiveMessages = true
            };
        }
    }
}