namespace PingPong.Shared
{
    public class TenantConfigurator : IGiveTenantConfiguration
    {
        private readonly string _readModelConnectionString;
        private readonly string _writeModelConnectionString;

        public TenantConfigurator(string readModelConnectionString, string writeModelConnectionString)
        {
            _readModelConnectionString = readModelConnectionString;
            _writeModelConnectionString = writeModelConnectionString;
        }

        public string GetReadModelConnectionString()
        {
            return _readModelConnectionString;
        }

        public string GetWriteModelConnectionString()
        {
            return _writeModelConnectionString;
        }

        public override string ToString()
        {
            return string.Format("ReadModel: {0} / WriteModel: {1}", _readModelConnectionString,
                _writeModelConnectionString);
        }
    }
}