using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace Ping.EntityFramework
{
    public class Configuration : DbConfiguration
    {
        public Configuration()
        {
            SetProviderServices(
                "System.Data.SqlClient",
                SqlProviderServices.Instance);
        }
    }
}