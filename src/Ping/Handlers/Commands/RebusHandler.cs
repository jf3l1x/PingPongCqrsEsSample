using System;
using Ping.Messages.Commands;
using Ping.Messages.Events;
using Ping.Messages.ExternalEvents;
using Rebus;

namespace Ping.Handlers.Commands
{
    internal class RebusHandler : IHandleMessages<StartPing>, IHandleMessages<StopPing>,
        IHandleMessages<ReceivePingResponse>, IHandleMessages<PingResponseReceived>, IHandleMessages<PingStarted>,
        IHandleMessages<PingStopped>, IHandleMessages<PongSent>
    {
        private readonly DefaultHandler _innerHandler;

        public RebusHandler(DefaultHandler innerHandler)
        {
            _innerHandler = innerHandler;
        }

        public void Handle(PingResponseReceived message)
        {
            _innerHandler.Handle(message);
        }

        public void Handle(PingStarted message)
        {
            _innerHandler.Handle(message);
        }

        public void Handle(PingStopped message)
        {
            _innerHandler.Handle(message);
        }

        public void Handle(PongSent message)
        {
            _innerHandler.Handle(message);
        }

        public void Handle(ReceivePingResponse message)
        {
            _innerHandler.Handle(message);
        }

        public void Handle(StartPing message)
        {
            _innerHandler.Handle(message);
        }

        public void Handle(StopPing message)
        {
            _innerHandler.Handle(message);
        }
    }
}