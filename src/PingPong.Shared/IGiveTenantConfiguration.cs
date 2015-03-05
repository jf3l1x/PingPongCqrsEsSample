namespace PingPong.Shared
{
    public interface IGiveTenantConfiguration
    {
        string GetReadModelConnectionString();
        string GetWriteModelConnectionString();
    }
}