using System;

namespace Pong.Messages.Commands
{
    public class GeneratePong : BaseMessage
    {
        public Guid PingId { get; set; }
        public DateTimeOffset RequestTime { get; set; }
    }
}
