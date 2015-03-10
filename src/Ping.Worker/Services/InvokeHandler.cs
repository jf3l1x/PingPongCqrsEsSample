using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Constant.Module.Interfaces.Bus;
using NEventStore;

namespace Ping.Worker.Services
{
    internal class InvokeHandler : IPipelineHook
    {
        private static readonly Dictionary<Type, MethodInfo> Creates = new Dictionary<Type, MethodInfo>();
        private static readonly Dictionary<Type, MethodInfo> Handles = new Dictionary<Type, MethodInfo>();
        private readonly MethodInfo _createMethod;
        private readonly IActivateHandlers _handlerFactory;

        public InvokeHandler(IActivateHandlers handlerFactory)
        {
            _handlerFactory = handlerFactory;
            _createMethod = _handlerFactory.GetType().GetMethod("GetHandlerInstancesFor");
        }

        public void Dispose()
        {
        }

        public ICommit Select(ICommit committed)
        {
            return committed;
        }

        public bool PreCommit(CommitAttempt attempt)
        {
            return true;
        }

        public void PostCommit(ICommit committed)
        {
            foreach (EventMessage evt in committed.Events)
            {
                Send(evt.Body);
            }
        }

        public void OnPurge(string bucketId)
        {
        }

        public void OnDeleteStream(string bucketId, string streamId)
        {
        }

        private void Send(object msg)
        {
            MethodInfo create = null;
            MethodInfo interfaceMethod = null;
            if (!Creates.TryGetValue(msg.GetType(), out create))
            {
                create = _createMethod.MakeGenericMethod(msg.GetType());

                Creates.Add(msg.GetType(), create);
            }
            var collection = (IEnumerable)create.Invoke(_handlerFactory, null);

            if (!Handles.TryGetValue(msg.GetType(), out interfaceMethod))
            {
                interfaceMethod = typeof(IHandleMessages<>).MakeGenericType(msg.GetType()).GetMethod("Handle", new[] { msg.GetType() });
                Handles.Add(msg.GetType(), interfaceMethod);
                
            }
            if (interfaceMethod != null)
            {
                foreach (var handler in collection)
                {
                    interfaceMethod.Invoke(handler, new[] { msg });
                } 
            }
           
            
        }
    }
}