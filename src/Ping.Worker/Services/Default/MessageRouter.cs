using System;
using Rebus;

namespace Ping.Worker.Services.Default
{
    internal class MessageRouter : IDetermineMessageOwnership
    {
        public string GetEndpointFor(Type messageType)
        {
            return "ping";
        }
    }
}