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
            // Mongo: "mongodb://localhost/testes"
            // SQL: Server=.;Database=pingpong;Trusted_Connection=True;

            var tenantConfiguratorPing = new TenantConfigurator("mongodb://localhost/testes", "Server=.;Database=pingpong;Trusted_Connection=True;");
            var configurationPing = new MemoryConfiguration(tenantConfiguratorPing, "amqp://jf3l1x:password@localhost:5672/testes") { ReceiveMessages = true };

            var tenantConfiguratorPong = new TenantConfigurator("Server=.;Database=pingpong;Trusted_Connection=True;", "Server=.;Database=pingpong;Trusted_Connection=True;");
            var configurationPong = new MemoryConfiguration(tenantConfiguratorPong, "amqp://jf3l1x:password@localhost:5672/testes") { ReceiveMessages = true };


            return new List<IModuleEngine>()
            {
                new Ping.Engine(configurationPing, new PingOptions
                {
                    RunMode = RunMode.Sync,
                    ReadModelPersistenceMode = ReadPersistenceMode.MongoDB,
                    WriteModelPersistenceMode = WritePersistenceMode.SqlServer
                }),
                new Pong.Engine(configurationPong, new PongOptions
                {
                    ReadModelPersistenceMode = ReadPersistenceMode.EntityFramework,
                    WriteModelPersistenceMode = WritePersistenceMode.SqlServer
                })
            };
        }

    }
}