using System.Collections;
using System.Collections.Generic;
using Constant.Module.Interfaces.Bus;
using Ping.Worker.Handlers;

namespace Ping.Worker.Services
{
    internal class HandlerActivator : IActivateHandlers
    {
        private readonly DefaultHandler _handler;

        public HandlerActivator(DefaultHandler handler)
        {
            _handler = handler;
        }

        public IEnumerable<IHandleMessages<T>> GetHandlerInstancesFor<T>()
        {
            var retval= _handler as IHandleMessages<T>;
            if (retval != null)
            {
                yield return retval;
            }
           
            
            
        }

        public void Release(IEnumerable handlerInstances)
        {
        }
    }
}