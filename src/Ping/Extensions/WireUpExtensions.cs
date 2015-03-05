using System.Net;
using EventStore.ClientAPI.SystemData;
using LightInject;
using NEventStore;
using NEventStore.Persistence.EventStore;
using NEventStore.Persistence.EventStore.Services;
using NEventStore.Persistence.EventStore.Services.Naming;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;
using NEventStore.Serialization;
using Ping.Configuration;
using PingPong.Shared;

namespace Ping.Extensions
{
    public static class WireUpExtensions
    {
        public static PersistenceWireup ConfigurePersistence(this Wireup target, PingOptions options,
            ServiceContainer container)
        {
            switch (options.WriteModelPersistenceMode)
            {
                case WritePersistenceMode.GetEventStore:
                    return target.UsingEventStorePersistence(
                        new EventStorePersistenceOptions
                        {
                            TcpeEndPoint = new IPEndPoint(IPAddress.Loopback, 1113),
                            HttpEndPoint = new IPEndPoint(IPAddress.Loopback, 2113),
                            UserCredentials = new UserCredentials("admin", "changeit"),
                            MinimunSnapshotThreshold = 50
                        }, new JsonNetSerializer(),
                        new DefaultNamingStrategy());

                case WritePersistenceMode.MongoDB:
                    return target.UsingMongoPersistence(() => container.GetInstance<IGiveTenantConfiguration>().GetWriteModelConnectionString(), new DocumentObjectSerializer());
                    
                default:
                    return target.UsingSqlPersistence(container.GetInstance<IConnectionFactory>())
                        .WithDialect(new MsSqlDialect());

            }
        }

    }
}