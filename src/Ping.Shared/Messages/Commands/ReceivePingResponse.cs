using System;

namespace Ping.Shared.Messages.Commands
{
    public class ReceivePingResponse:BaseMessage
    {
        public DateTimeOffset ResponseTime { get; set; }
    }
}