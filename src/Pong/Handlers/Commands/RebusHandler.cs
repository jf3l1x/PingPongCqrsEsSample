using Pong.Messages.Commands;
using Pong.Messages.Events;
using Pong.Messages.ExternalEvents;
using Rebus;

namespace Pong.Handlers.Commands
{
    internal class RebusHandler : IHandleMessages<PongRequested>,IHandleMessages<GeneratePong>,IHandleMessages<PongGenerated>
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

        public void Handle(GeneratePong message)
        {
            _innerHandler.Handle(message);
        }

        public void Handle(PongGenerated message)
        {
            _innerHandler.Handle(message);
        }
    }
}
