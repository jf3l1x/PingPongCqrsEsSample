using Ping.Handlers.Commands;

namespace Ping.Handlers
{
    public class SynchronousHandlerFactory : ICreateHandle
    {
        private readonly CmdHandler _handler;

        public SynchronousHandlerFactory(CmdHandler handler)
        {
            _handler = handler;
        }

        public IHandle<T> Create<T>()
        {
            return (IHandle<T>) _handler;
        }
    }
}