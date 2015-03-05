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
            var tenantConfigurator = new TenantConfigurator("Server=.;Database=pingpong;Trusted_Connection=True;");
            var configuration = new MemoryConfiguration(tenantConfigurator, "amqp://jf3l1x:password@localhost:5672/testes");

            app.Map("/ping", map => new Ping.Engine(configuration,
                new PingOptions(RunMode.Async, WritePersistenceMode.SqlServer, ReadPersistenceMode.NHibernate))
                .RegisterApi(map));

            app.Map("/pong", map => new Pong.Engine(configuration, 
                new PongOptions())
                .RegisterApi(map));
        }
    }
}
