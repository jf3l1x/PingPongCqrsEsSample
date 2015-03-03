using System;

namespace Pong.Messages.ExternalEvents
{
    public class PongSent : BaseMessage
    {
        public Guid PingId { get; set; }
        public DateTimeOffset SendTime { get; set; }
    }
}