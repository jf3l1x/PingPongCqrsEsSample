using Ping.Shared.Messages.Commands;
using PingPong.Shared;

namespace Ping.Worker.Handlers.Async
{
    internal class AsynchronousHandler : ICreateHandlers, IHandle<ReceivePingResponse>, IHandle<StartPing>,
        IHandle<StopPing>
    {
        private readonly IServiceBus _bus;

        public AsynchronousHandler(IServiceBus bus)
        {
            _bus = bus;
        }

        public IHandle<T> Create<T>()
        {
            return (IHandle<T>) this;
        }

        public void Handle(ReceivePingResponse msg)
        {
            _bus.Send(msg);
        }

        public void Handle(StartPing msg)
        {
            _bus.Send(msg);
        }

        public void Handle(StopPing msg)
        {
            _bus.Send(msg);
        }
    }
}