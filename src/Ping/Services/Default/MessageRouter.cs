using System;
using System.Collections.Generic;
using Ping.Messages.Commands;
using Ping.Messages.Events;
using Ping.Messages.ExternalEvents;
using Rebus;

namespace Ping.Services.Default
{
    internal class MessageRouter : IDetermineMessageOwnership
    {
        public string GetEndpointFor(Type messageType)
        {
            return "ping";
        }
    }
}