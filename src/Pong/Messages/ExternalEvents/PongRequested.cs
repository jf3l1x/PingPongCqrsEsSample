using System;

namespace Pong.Messages.ExternalEvents
{
    public class PongRequested
    {
        public Guid PingId { get; set; }
        public DateTimeOffset RequestTime { get; set; }
    }
}
