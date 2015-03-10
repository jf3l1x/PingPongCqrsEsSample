using System;
using Constant.Module.Interfaces.Bus;

namespace Ping.Web.Services.Default
{
    internal class MessageRouter : IRouteMessages
    {
        public string DestinationFor(Type t)
        {
            return "ping";
        }
    }
}