using Constant.Module.Interfaces.Bus;
using Newtonsoft.Json;

namespace Ping.Shared
{
    public class DefaultMessageMutator : IMutateMessages
    {
        private readonly IResolveTypeName _resolver;

        public DefaultMessageMutator(IResolveTypeName resolver)
        {
            _resolver = resolver;
        }

        public object MutateIncoming(object message)
        {
            var busMessage = message as BusMessage;
            if (busMessage != null)
            {
                return JsonConvert.DeserializeObject(busMessage.Content, _resolver.Resolve(busMessage.EventName));
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
}