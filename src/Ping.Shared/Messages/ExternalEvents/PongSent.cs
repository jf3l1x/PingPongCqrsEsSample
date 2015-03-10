using System;

namespace Ping.Shared.Messages.ExternalEvents
{
    public class PongSent : BaseMessage
    {
        public Guid PingId { get; set; }
        public DateTimeOffset SendTime { get; set; }
    }
}