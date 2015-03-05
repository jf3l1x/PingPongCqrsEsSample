using System;
using System.Collections.Generic;
using System.Linq;
using Ping.Configuration;
using PingPong.Shared;
using Pong.Configuration;


namespace PingPong.Worker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CreateModules().ToList().ForEach(m => m.StartListener());
            Console.WriteLine("Press some key to end!");
            Console.Read();
        }

        private static IEnumerable<IModuleEngine> CreateModules()
        {
            var tenantConfigurator = new TenantConfigurator("Server=.;Database=pingpong;Trusted_Connection=True;");
            var configuration = new MemoryConfiguration(tenantConfigurator, "amqp://jf3l1x:password@localhost:5672/testes") { ReceiveMessages = true };

            return new List<IModuleEngine>()
            {
                new Ping.Engine(configuration, new PingOptions
                {
                    RunMode = RunMode.Sync,
                    ReadModelPersistenceMode = ReadPersistenceMode.EntityFramework,
                    WriteModelPersistenceMode = WritePersistenceMode.SqlServer
                }),
                new Pong.Engine(configuration, new PongOptions
                {
                    ReadModelPersistenceMode = ReadPersistenceMode.EntityFramework,
                    WriteModelPersistenceMode = WritePersistenceMode.SqlServer
                })
            };
        }

    }
}