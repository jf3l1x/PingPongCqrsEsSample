using System;

namespace Ping.Messages.Commands
{
    public class StartPing:BaseMessage
    {
        public TimeSpan TimeLimit { get; set; }
        public int CountLimit { get; set; }
    }
}