using System;
using System.Net;

using LightInject;
using NEventStore;
using NEventStore.Persistence.EventStore;
using NEventStore.Persistence.EventStore.Services;
using NEventStore.Persistence.EventStore.Services.Naming;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;
using NEventStore.Serialization;
using PingPong.Shared;
using Pong.Configuration;

namespace Ping.Extensions
{
    public static class WireUpExtensions
    {
        public static PersistenceWireup ConfigurePersistence(this Wireup target, PongOptions options,
            ServiceContainer container)
        {
            switch (options.WriteModelPersistenceMode)
            {
               
                    
                default:
                    return target.UsingSqlPersistence(container.GetInstance<IConnectionFactory>())
                        .WithDialect(new MsSqlDialect());
            }
        }
    }
}