using Owin;
﻿using Ping.Configuration;
﻿using PingPong.Shared;
using Pong.Configuration;

namespace PingPong.WebHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Mongo: "mongodb://localhost/testes"
            // SQL: Server=.;Database=pingpong;Trusted_Connection=True;

            var tenantConfiguratorPing = new TenantConfigurator("mongodb://localhost/testes", "Server=.;Database=pingpong;Trusted_Connection=True;");
            var configurationPing = new MemoryConfiguration(tenantConfiguratorPing, "amqp://jf3l1x:password@localhost:5672/testes") { ReceiveMessages = true };

            var tenantConfiguratorPong = new TenantConfigurator("Server=.;Database=pingpong;Trusted_Connection=True;", "Server=.;Database=pingpong;Trusted_Connection=True;");
            var configurationPong = new MemoryConfiguration(tenantConfiguratorPong, "amqp://jf3l1x:password@localhost:5672/testes") { ReceiveMessages = true };

            app.Map("/ping", map => new Ping.Engine(configurationPing,
                new PingOptions
            {
                RunMode = RunMode.Async, 
                WriteModelPersistenceMode = WritePersistenceMode.SqlServer, 
                ReadModelPersistenceMode = ReadPersistenceMode.MongoDB
            }).RegisterApi(map));

            app.Map("/pong", map => new Pong.Engine(configurationPong, 
                new PongOptions())
                .RegisterApi(map));
        }
    }
}
