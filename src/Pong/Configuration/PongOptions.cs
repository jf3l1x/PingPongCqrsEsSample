using PingPong.Shared;

namespace Pong.Configuration
{
    public class PongOptions
    {
        public WritePersistenceMode WriteModelPersistenceMode { get; set; }
        public ReadPersistenceMode ReadModelPersistenceMode { get; set; }
    }
}