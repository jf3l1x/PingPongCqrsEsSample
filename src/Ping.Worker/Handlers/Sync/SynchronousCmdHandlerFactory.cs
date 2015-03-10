using Ping.Worker.Handlers.Commands;

namespace Ping.Worker.Handlers.Sync
{
    public class SynchronousCmdHandlerFactory : ICreateHandlers
    {
        private readonly DefaultHandler _handler;


        public SynchronousCmdHandlerFactory(DefaultHandler handler)
        {
            _handler = handler;
        }

        public IHandle<T> Create<T>()
        {
            return (IHandle<T>)_handler;
        }
    }
}