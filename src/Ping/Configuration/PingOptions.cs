using PingPong.Shared;

namespace Ping.Configuration
{
    public class PingOptions
    {
        public RunMode RunMode { get; set; }
        public WritePersistenceMode WriteModelPersistenceMode { get; set; }
        public ReadPersistenceMode ReadModelPersistenceMode { get; set; }
    }
}