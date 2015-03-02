namespace PingPong.Shared
{
    public interface IModuleConfiguration
    {
        IGiveTenantConfiguration TenantConfigurator { get; }
        string BusConnectionString { get; }
    }
}