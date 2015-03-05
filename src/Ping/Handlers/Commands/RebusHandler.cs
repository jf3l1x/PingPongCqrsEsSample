using Ping.Messages.Commands;
using Ping.Messages.Events;
using Ping.Messages.ExternalEvents;
using Rebus;

namespace Ping.Handlers.Commands
{
    public class RebusHandler : IHandleMessages<StartPing>, IHandleMessages<StopPing>,
        IHandleMessages<ReceivePingResponse>, IHandleMessages<PingResponseReceived>, IHandleMessages<PingStarted>,
        IHandleMessages<PingStopped>, IHandleMessages<PongSent>
    {
        private readonly DefaultHandler _innerHandler;
        

        public RebusHandler(DefaultHandler innerHandler)
        {
            _innerHandler = innerHandler;
        }

        #region Events

        public virtual void Handle(PingResponseReceived message)
        {
            _innerHandler.Handle(message);
        }

        public virtual void Handle(PingStarted message)
        {
            _innerHandler.Handle(message);
        }

        public virtual void Handle(PingStopped message)
        {
            _innerHandler.Handle(message);
        }

        public virtual void Handle(PongSent message)
        {
            _innerHandler.Handle(message);
        }

        #endregion
        
        #region Command

        public virtual void Handle(StartPing message)
        {
            _innerHandler.Handle(message);
        }

        public virtual void Handle(StopPing message)
        {
            _innerHandler.Handle(message);
        }

        public virtual void Handle(ReceivePingResponse message)
        {
            _innerHandler.Handle(message);
        }

        #endregion

    }

}