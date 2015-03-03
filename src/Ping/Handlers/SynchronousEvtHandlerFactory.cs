using Ping.Handlers.Events;

namespace Ping.Handlers
{
    internal class SynchronousEvtHandlerFactory:ICreateEvtHandle
    {
        private readonly EvtHandler _evtHandler;


        public SynchronousEvtHandlerFactory(EvtHandler evtHandler)
        {
            _evtHandler = evtHandler;
        }

        public IHandle<T> Create<T>()
        {
            return (IHandle<T>) _evtHandler;
        }
    }
}