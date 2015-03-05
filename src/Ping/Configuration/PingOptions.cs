using PingPong.Shared;

namespace Ping.Configuration
{
    public class PingOptions
    {
        public PingOptions()
        {
            
        }

        public PingOptions(RunMode runMode, WritePersistenceMode writeMode, ReadPersistenceMode readMode)
        {
            RunMode = runMode;
            WriteModelPersistenceMode = writeMode;
            ReadModelPersistenceMode = readMode;
        }

        public RunMode RunMode { get; set; }
        public WritePersistenceMode WriteModelPersistenceMode { get; set; }
        public ReadPersistenceMode ReadModelPersistenceMode { get; set; }
    }
}