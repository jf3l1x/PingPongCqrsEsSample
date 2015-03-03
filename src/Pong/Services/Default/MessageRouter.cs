using System;
using Rebus;

namespace Pong.Services.Default
{
    internal class MessageRouter : IDetermineMessageOwnership
    {
        public string GetEndpointFor(Type messageType)
        {
            return "pong";
        }
    }
}