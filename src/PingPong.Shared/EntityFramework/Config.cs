using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace PingPong.Shared.EntityFramework
{
    public class Config : DbConfiguration
    {
        public Config()
        {
            SetProviderServices(
                "System.Data.SqlClient",
                SqlProviderServices.Instance);
        }
    }
}