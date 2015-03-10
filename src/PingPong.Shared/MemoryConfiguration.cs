using System.Text;
using Constant.Module.Interfaces.Configuration;

namespace PingPong.Shared
{
    public class MemoryConfiguration : IModuleConfiguration
    {
        public MemoryConfiguration(IGiveTenantConfiguration tenantConfigurator)
        {
            TenantConfigurator = tenantConfigurator;
            BusConnectionString = "amqp://jf3l1x:password@localhost:5672/testes";
        }

        public IGiveTenantConfiguration TenantConfigurator { get; private set; }
        public string BusConnectionString { get; private set; }
        public bool ReceiveMessages { get; set; }

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