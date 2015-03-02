using System;

namespace Ping.Messages.Events
{
    public class PingStopped : BaseMessage
    {
        
        public DateTimeOffset StopTime { get; set; }
    }
}