using System;
using Newtonsoft.Json;
using Rebus;

namespace PingPong.Shared
{
    public class DefaultMessageMutator : IMutateMessages
    {
        private readonly IResolveTypes _resolver;

        public DefaultMessageMutator(IResolveTypes resolver)
        {
            _resolver = resolver;
        }

        public object MutateIncoming(object message)
        {
            var busMessage = message as BusMessage;
            if (busMessage != null)
            {
                return JsonConvert.DeserializeObject(busMessage.Content, _resolver.ResolveType(busMessage.EventName));
            }
            return null;
        }

        public object MutateOutgoing(object message)
        {
            return new BusMessage
            {
                Content = JsonConvert.SerializeObject(message),
                EventName = message.GetType().Name
            };
        }
    }

    public interface IResolveTypes
    {
        Type ResolveType(string eventName);
    }
}