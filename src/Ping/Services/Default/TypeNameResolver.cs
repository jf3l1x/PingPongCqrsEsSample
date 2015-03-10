using System;
using Constant.Module.Interfaces.Bus;
using Ping.Shared.Messages.ExternalEvents;

namespace Ping.Web.Services.Default
{
    internal class TypeNameResolver : IResolveTypeName
    {
        public string Resolve(Type t)
        {
            if (t == typeof (PongRequested))
                return "pongrequested";
            if (t == typeof (PongSent))
                return "pongsent";
            return string.Empty;
        }

        public Type Resolve(string name)
        {
            if (name == "pongrequested")
                return typeof(PongRequested);
            if (name == "pongsent")
                return typeof(PongSent);
            return typeof (object);
        }
    }
}