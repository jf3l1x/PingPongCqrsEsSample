using System;

namespace Ping.Messages.Events
{
    public class PingStarted : BaseMessage
    {
        
        public DateTimeOffset StartTime { get; set; }
        public TimeSpan TimeLimit { get; set; }
        public int CountLimit { get; set; }
    }
}