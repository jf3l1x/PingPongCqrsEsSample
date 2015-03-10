using System;
using PingPong.Shared;
using Rebus;

namespace Ping.Worker.Configuration
{
    public class PingOptions
    {
        public RunMode RunMode { get; set; }
        public WritePersistenceMode WriteModelPersistenceMode { get; set; }
        public ReadPersistenceMode ReadModelPersistenceMode { get; set; }
        public Func<string, IBus> ConfigureTransport { get; set; }
    }
}