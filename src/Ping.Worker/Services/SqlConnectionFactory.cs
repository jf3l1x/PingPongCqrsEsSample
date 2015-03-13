using System;
using System.Data;
using System.Data.SqlClient;
using Constant.Module.Interfaces.Configuration;
using NEventStore.Persistence.Sql;

namespace Ping.Worker.Services
{
    public class SqlConnectionFactory : IConnectionFactory
    {
        private readonly Func<IGiveTenantConfiguration> _configuratorFactory;
        

        public SqlConnectionFactory(Func<IGiveTenantConfiguration> configuratorFactory)
        {
            _configuratorFactory = configuratorFactory;
            
        }

        public IDbConnection Open()
        {
            var connection = new SqlConnection(_configuratorFactory().GetReadModelConnectionString());
            connection.Open();
            return connection;
        }

        public Type GetDbProviderFactoryType()
        {
            return typeof (SqlClientFactory);
        }
    }
}