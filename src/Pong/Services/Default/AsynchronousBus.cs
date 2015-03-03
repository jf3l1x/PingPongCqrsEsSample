using PingPong.Shared;
using Rebus;

namespace Pong.Services.Default
{
    internal class AsynchronousBus : IServiceBus
    {
        private readonly IBus _bus;

        public AsynchronousBus(IBus bus)
        {
            _bus = bus;
        }

        public void Send(object msg)
        {
            _bus.Send(msg);
        }

        public void Publish(object msg)
        {
            _bus.Publish(msg);
        }
    }
}
