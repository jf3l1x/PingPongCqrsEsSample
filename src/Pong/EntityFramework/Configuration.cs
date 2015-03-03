using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace Pong.EntityFramework
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
