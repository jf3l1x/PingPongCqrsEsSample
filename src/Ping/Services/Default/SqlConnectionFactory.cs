using System.Data;
using System.Data.SqlClient;

namespace Ping.Web.Services.Default
{
    internal class SqlConnectionFactory : IConnectionFactory
    {
        public IDbConnection Open()
        {
            var connection = new SqlConnection("Server=.;Database=pingpong;Trusted_Connection=True;");
            connection.Open();
            return connection;
        }
    }
}