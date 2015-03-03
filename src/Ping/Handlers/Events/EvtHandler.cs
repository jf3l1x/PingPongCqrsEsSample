using System;
using Ping.Messages.Events;
using Ping.Messages.ExternalEvents;
using Ping.Model.Read;
using PingPong.Shared;

namespace Ping.Handlers.Events
{
    public class EvtHandler : IHandle<PingResponseReceived>, IHandle<PingStarted>, IHandle<PingStopped>,
        IHandle<PongSent>
    {
        private readonly IReadModelRepository<PingSummary> _repository;


        public EvtHandler(IReadModelRepository<PingSummary> repository)
        {
            _repository = repository;
        }

        public void Handle(PingResponseReceived evt)
        {
            Console.WriteLine("eventHandled:{0}", evt.GetType().Name);
        }

        public void Handle(PingStarted evt)
        {
            PingSummary summary = _repository.Retrieve(evt.AggregateId);
            if (summary == null)
            {
                summary = new PingSummary
                {
                    Active = true,
                    Id = evt.AggregateId,
                    PingsPerSecond = 0,
                    Start = evt.StartTime,
                    TotalResponses = 0
                };
                _repository.Create(summary);
            }
            else
            {
                summary.Start = evt.StartTime;
                summary.PingsPerSecond = 0;
                summary.End = null;
                summary.TotalResponses = 0;
                _repository.Update(summary);
            }
        }

        public void Handle(PingStopped evt)
        {
            Console.WriteLine("eventHandled:{0}", evt.GetType().Name);
        }

        public void Handle(PongSent evt)
        {
            Console.WriteLine("eventHandled:{0}", evt.GetType().Name);
        }
    }
}