namespace PingPong.Shared
{
    public interface IModuleConfiguration
    {
        IGiveTenantConfiguration TenantConfigurator { get; }
        string BusConnectionString { get; }
        bool ReceiveMessages { get; set; }
    }
}