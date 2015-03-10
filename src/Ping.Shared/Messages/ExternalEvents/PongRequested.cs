using System;

namespace Ping.Shared.Messages.ExternalEvents
{
    public class PongRequested : BaseMessage
    {
        
        public DateTimeOffset RequestTime { get; set; }
    }
}