using System;
using PingPong.Shared;
using Pong.Messages.Commands;

namespace Pong.Handlers.Async
{
    internal class AsynchronousHandler : ICreateHandlers, IHandle<GeneratePong>
    {
        private readonly IServiceBus _bus;

        public AsynchronousHandler(IServiceBus bus)
        {
            _bus = bus;
        }

        public IHandle<T> Create<T>()
        {
            return (IHandle<T>)this;
        }

        public void Handle(GeneratePong msg)
        {
            _bus.Send(msg);
        }
    }
}
