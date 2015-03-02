using System.Text;

namespace PingPong.Shared
{
    public class MemoryConfiguration : IModuleConfiguration
    {
        public MemoryConfiguration(IGiveTenantConfiguration tenantConfigurator, string busConnectionString)
        {
            TenantConfigurator = tenantConfigurator;
            BusConnectionString = busConnectionString;
        }

        public IGiveTenantConfiguration TenantConfigurator { get; private set; }
        public string BusConnectionString { get; private set; }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Bus Connection String : {0} ", BusConnectionString);
            sb.AppendLine();
            sb.AppendLine(TenantConfigurator.ToString());
            return sb.ToString();

        }
    }
}