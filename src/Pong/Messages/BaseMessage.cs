using System;

namespace Pong.Messages
{
    public class BaseMessage
    {
        public BaseMessage()
        {
            AggregateId = Guid.NewGuid();
        }

        public Guid AggregateId { get; set; }
    }
}