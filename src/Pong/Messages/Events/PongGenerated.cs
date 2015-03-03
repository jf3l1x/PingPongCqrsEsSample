using System;

namespace Pong.Messages.Events
{
    public class PongGenerated : BaseMessage
    {
        public Guid PingId { get; set; }
        public DateTimeOffset RequestTime { get; set; }
    }
}
