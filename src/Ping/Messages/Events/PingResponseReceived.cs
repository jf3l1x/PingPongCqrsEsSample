using System;

namespace Ping.Messages.Events
{
    public class PingResponseReceived : BaseMessage
    {
        
        public DateTimeOffset ResponseTime { get; set; }
        public DateTimeOffset ReceiveTime { get; set; }
    }
}