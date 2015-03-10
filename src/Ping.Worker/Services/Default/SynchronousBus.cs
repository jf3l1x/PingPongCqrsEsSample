using System;
using System.Collections.Generic;
using System.Reflection;
using PingPong.Shared;
using Rebus;

namespace Ping.Worker.Services.Default
{
    public class SynchronousBus : IServiceBus
    {
        private readonly ICreateHandlers _handlerFactory;
        private static readonly Dictionary<Type, MethodInfo> Creates = new Dictionary<Type, MethodInfo>();
        private static readonly Dictionary<Type, MethodInfo> Handles = new Dictionary<Type, MethodInfo>();
        private readonly MethodInfo _createMethod;
        private Lazy<IBus> _innerBus;

        public SynchronousBus(ICreateHandlers handlerFactory,Func<IBus> busFactory )
        {
            _handlerFactory = handlerFactory;
            _createMethod = _handlerFactory.GetType().GetMethod("Create");
            _innerBus = new Lazy<IBus>(busFactory);

        }
        
        public void Send(object msg)
        {
            MethodInfo create=null;
            MethodInfo handle=null;
            if (!Creates.TryGetValue(msg.GetType(),out create))
            {
                create = _createMethod.MakeGenericMethod(msg.GetType());
               
                Creates.Add(msg.GetType(),create);
                
            }
            var handler = create.Invoke(_handlerFactory,null);
            if (!Handles.TryGetValue(msg.GetType(), out handle))
            {
                handle=handler.GetType().GetMethod("Handle",new []{msg.GetType()});
                Handles.Add(msg.GetType(), handle);
            }
            
            handle.Invoke(handler, new []{msg});
        }

        public void Publish(object msg)
        {
            _innerBus.Value.Publish(msg);
        }

       
    }
}