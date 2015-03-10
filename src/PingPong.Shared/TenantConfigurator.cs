using System;

namespace PingPong.Shared
{
    [Serializable]
    public class TenantConfigurator : Constant.Module.Interfaces.Configuration.IGiveTenantConfiguration
    {
        public T GetValue<T>(string name)
        {
            return default(T);
        }

        public object GetValue(string name)
        {
            return null;
        }

        public string GetReadModelConnectionString()
        {
            return "Server=.;Database=pingpong;Trusted_Connection=True;";
        }

        public string GetWriteModelConnectionString()
        {
            return "Server=.;Database=pingpong;Trusted_Connection=True;";
        }

        public string GetBusConnectionString()
        {
            return "amqp://jf3l1x:password@localhost:5672/testes";
        }
    }
}