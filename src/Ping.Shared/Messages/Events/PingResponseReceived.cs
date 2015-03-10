using System;

namespace Ping.Shared.Messages.Events
{
    public class PingResponseReceived : BaseMessage
    {
        
        public DateTimeOffset ResponseTime { get; set; }
        public DateTimeOffset ReceiveTime { get; set; }
    }
}