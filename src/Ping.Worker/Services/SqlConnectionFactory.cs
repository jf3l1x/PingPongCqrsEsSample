using System;
using System.Data;
using System.Data.SqlClient;
using Constant.Module.Interfaces.Configuration;
using NEventStore.Persistence.Sql;

namespace Ping.Worker.Services
{
    public class SqlConnectionFactory : IConnectionFactory
    {
        private readonly IGiveTenantConfiguration _configurator;

        public SqlConnectionFactory(IGiveTenantConfiguration configurator)
        {
            _configurator = configurator;
        }

        public IDbConnection Open()
        {
            var connection = new SqlConnection(_configurator.GetReadModelConnectionString());
            connection.Open();
            return connection;
        }

        public Type GetDbProviderFactoryType()
        {
            return typeof (SqlClientFactory);
        }
    }
}