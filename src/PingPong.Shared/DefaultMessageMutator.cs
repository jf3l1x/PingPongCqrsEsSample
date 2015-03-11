using System;
using Constant.Module.Interfaces.Bus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IMutateMessages = Rebus.IMutateMessages;

namespace PingPong.Shared
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
            var busMessage = message as string;
            if (busMessage != null)
            {
                var json = JObject.Parse(busMessage);
                return json.ToObject(_resolver.Resolve(json.Value<string>("eventName")));
            }
            return null;
        }

        public object MutateOutgoing(object message)
        {

            var json = JObject.FromObject(message);
            json.Add("eventName", _resolver.Resolve(message.GetType()));
            return json.ToString();
            ;
        }

    }

   
}