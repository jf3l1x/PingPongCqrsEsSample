using System;

namespace Ping.Messages.Commands
{
    public class ReceivePingResponse:BaseMessage
    {
        public DateTimeOffset ResponseTime { get; set; }
    }
}