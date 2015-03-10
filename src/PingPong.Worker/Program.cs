using System;
using Constant.Module.Interfaces;
using Constant.Module.Interfaces.Configuration;
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
            StartInAnotherAppDomain(typeof(Ping.Worker.Engine),container.GetInstance<IWorkerModuleContainer>());

            Console.WriteLine("Press some key to end!");
            Console.Read();
        }

        private static void StartInAnotherAppDomain(Type t,IWorkerModuleContainer container)
        {
            var appDomain = AppDomain.CreateDomain(t.FullName);
            var module=(IWorkerModule)appDomain.CreateInstanceAndUnwrap(t.Assembly.FullName,
                t.FullName);
            module.Start(container);
        }
        private static ServiceContainer CreateInjectorContainer()
        {
            var container = new ServiceContainer();
            container.Register<IGiveTenantConfiguration, Shared.TenantConfigurator>();
            container.Register<IWorkerModuleContainer,WorkerModuleContainer>();
            container.Register<IModuleConfiguration,MemoryConfiguration>();
            
            container.Register<IModuleEngine, Engine>("pong");

            container.RegisterInstance(new PongOptions
            {
                ReadModelPersistenceMode = ReadPersistenceMode.EntityFramework,
                WriteModelPersistenceMode = WritePersistenceMode.SqlServer
            });

            return container;
        }

       
    }
}