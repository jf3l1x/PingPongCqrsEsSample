using System;

namespace Pong.Messages.ExternalEvents
{
    public class PongRequested:BaseMessage
    {

        public DateTimeOffset RequestTime { get; set; }
    }
}
