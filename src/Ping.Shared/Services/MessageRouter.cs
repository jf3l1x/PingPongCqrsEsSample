using System;
using Constant.Module.Interfaces.Bus;

namespace Ping.Shared.Services
{
    public class MessageRouter : IRouteMessages
    {
        public string DestinationFor(Type t)
        {
            return "ping";
        }
    }
}