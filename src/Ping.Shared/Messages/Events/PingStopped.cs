using System;

namespace Ping.Shared.Messages.Events
{
    public class PingStopped : BaseMessage
    {
        
        public DateTimeOffset StopTime { get; set; }
    }
}