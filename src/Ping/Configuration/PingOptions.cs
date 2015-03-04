﻿namespace Ping.Configuration
{
    public class PingOptions
    {
        public RunMode RunMode { get; set; }
        public PersistenceMode WriteModelPersistenceMode { get; set; }
        public PersistenceMode ReadModelPersistenceMode { get; set; }
    }
}