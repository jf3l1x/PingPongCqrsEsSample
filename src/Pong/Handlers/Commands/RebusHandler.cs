using Pong.Messages.ExternalEvents;
using Rebus;

namespace Pong.Handlers.Commands
{
    internal class RebusHandler : IHandleMessages<PongRequested>
    {
        private readonly DefaultHandler _innerHandler;

        public RebusHandler(DefaultHandler innerHandler)
        {
            _innerHandler = innerHandler;
        }

        public void Handle(PongRequested message)
        {
            _innerHandler.Handle(message);
        }
    }
}
