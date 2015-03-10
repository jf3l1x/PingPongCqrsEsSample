using LightInject;
using NEventStore;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;
using Ping.Worker.Configuration;

namespace Ping.Worker.Extensions
{
    public static class WireUpExtensions
    {
        public static PersistenceWireup ConfigurePersistence(this Wireup target, PingOptions options,
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