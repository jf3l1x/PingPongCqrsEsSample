using System.Collections.Generic;
using System.Text;

namespace PingPong.Shared
{
    public class TenantConfigurator : IGiveTenantConfiguration
    {
        private readonly Dictionary<string, string> _tenants;

        public TenantConfigurator()
        {
            _tenants = new Dictionary<string, string>();
        }

        public string GetConnectionString(string tenantId)
        {
            return _tenants[tenantId];
        }

        public void RegisterTenant(string id, string connectionString)
        {
            _tenants.Add(id, connectionString);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var tenant in _tenants)
            {
                sb.AppendFormat("{0}:{1}", tenant.Key, tenant.Value);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}