using System;

namespace Ping.Messages.ExternalEvents
{
    public class PongSent : BaseMessage
    {
        public DateTimeOffset SendTime { get; set; }
    }
}