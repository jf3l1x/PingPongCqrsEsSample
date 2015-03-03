using System.Collections.Generic;
using System.Text;

namespace PingPong.Shared
{
    public class TenantConfigurator : IGiveTenantConfiguration
    {
        private readonly string _connectionString;


        public TenantConfigurator(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }

      

        public override string ToString()
        {
            
           
                return string.Format("{0}:{1}", "default", _connectionString);
            
        }
    }
}