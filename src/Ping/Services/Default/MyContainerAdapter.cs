using System;
using System.Collections;
using System.Collections.Generic;
using Ping.Handlers;
using Ping.Handlers.Commands;
using Rebus;
using Rebus.Configuration;

namespace Ping.Services.Default
{
    internal class MyContainerAdapter : IContainerAdapter
    {
        private readonly RebusHandler _handler;

        private IBus _bus;

        public MyContainerAdapter(RebusHandler handler)
        {
            _handler = handler;
            
        }

        public IEnumerable<IHandleMessages> GetHandlerInstancesFor<T>()
        {
            yield return _handler;
        }

        public void Release(IEnumerable handlerInstances)
        {
        }

        public void SaveBusInstances(IBus bus)
        {
            if (!ReferenceEquals(null, _bus))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "You can't call SaveBusInstances twice on the container adapter! Already have bus instance {0} when you tried to overwrite it with {1}",
                        _bus, bus));
            }

            _bus = bus;
        }
    }
}