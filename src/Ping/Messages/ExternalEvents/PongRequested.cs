using System;

namespace Ping.Messages.ExternalEvents
{
    public class PongRequested : BaseMessage
    {
        
        public DateTimeOffset RequestTime { get; set; }
    }
}