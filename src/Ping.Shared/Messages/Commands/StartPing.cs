using System;

namespace Ping.Shared.Messages.Commands
{
    public class StartPing:BaseMessage
    {
        public TimeSpan TimeLimit { get; set; }
        public int CountLimit { get; set; }
    }
}