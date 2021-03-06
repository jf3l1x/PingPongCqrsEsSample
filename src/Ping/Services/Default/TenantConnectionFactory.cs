using System;
using System.Data;
using System.Data.SqlClient;
using NEventStore.Persistence.Sql;
using PingPong.Shared;

namespace Ping.Services.Default
{
    internal class TenantConnectionFactory:IConnectionFactory
    {
        private readonly IGiveTenantConfiguration _tenantConfigurator;

        public TenantConnectionFactory(IGiveTenantConfiguration tenantConfigurator)
        {
            _tenantConfigurator = tenantConfigurator;
            
        }

        public IDbConnection Open()
        {
            var connection = new SqlConnection(_tenantConfigurator.GetWriteModelConnectionString());
            connection.Open();
            return connection;
        }

        public Type GetDbProviderFactoryType()
        {
            return typeof(SqlClientFactory);
        }
    }
}