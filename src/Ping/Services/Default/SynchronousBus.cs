using System;
using System.Collections.Generic;
using System.Reflection;
using Ping.Handlers;
using PingPong.Shared;

namespace Ping.Services.Default
{
    public class SynchronousBus : IServiceBus
    {
        private readonly ICreateEvtHandle _handlerFactory;
        private static readonly Dictionary<Type, MethodInfo> Creates = new Dictionary<Type, MethodInfo>();
        private static readonly Dictionary<Type, MethodInfo> Handles = new Dictionary<Type, MethodInfo>();
        private readonly MethodInfo _createMethod;
        
        public SynchronousBus(ICreateEvtHandle handlerFactory)
        {
            _handlerFactory = handlerFactory;
            _createMethod = _handlerFactory.GetType().GetMethod("Create");
            
            
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
            Console.WriteLine(msg.GetType().Name);
        }

       
    }
}