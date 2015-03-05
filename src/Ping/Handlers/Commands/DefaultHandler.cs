using System;
using CommonDomain.Core;
using CommonDomain.Persistence;
using Ping.Messages.Commands;
using Ping.Messages.Events;
using Ping.Messages.ExternalEvents;
using Ping.Model.Domain;
using Ping.Model.Read;
using PingPong.Shared;

namespace Ping.Handlers.Commands
{
    public class DefaultHandler : IHandle<StartPing>, IHandle<StopPing>, IHandle<ReceivePingResponse>,
        IHandle<PingResponseReceived>, IHandle<PingStarted>, IHandle<PingStopped>,IHandle<PongSent>
    {
        private readonly Func<IRepository> _writeRepositoryFactory;
        private readonly Lazy<IServiceBus> _bus;
        private readonly Lazy<IReadModelRepository<PingSummary>> _readModelRepository;
        
        public DefaultHandler(Func<IRepository> writeRepositoryFactory, Func<IServiceBus> busFactory,
            Func<IReadModelRepository<PingSummary>> readRepositoryFactory)
        {
            _writeRepositoryFactory = writeRepositoryFactory;
            _bus = new Lazy<IServiceBus>(busFactory);
            _readModelRepository = new Lazy<IReadModelRepository<PingSummary>>(readRepositoryFactory);
        }

        public void Handle(PingResponseReceived evt)
        {
            PingSummary summary = _readModelRepository.Value.Retrieve(evt.AggregateId);
            if (summary != null)
            {
                summary.TotalResponses++;
                summary.PingsPerSecond = summary.TotalResponses/
                                         DateTimeOffset.UtcNow.Subtract(summary.Start).TotalSeconds;
                _readModelRepository.Value.Update(summary);
            }
        }

        public void Handle(PingStarted evt)
        {
            PingSummary summary = _readModelRepository.Value.Retrieve(evt.AggregateId);
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
                _readModelRepository.Value.Create(summary);
            }
            else
            {
                summary.Start = evt.StartTime;
                summary.PingsPerSecond = 0;
                summary.End = null;
                summary.TotalResponses = 0;
                _readModelRepository.Value.Update(summary);
            }
        }

        public void Handle(PingStopped evt)
        {
            PingSummary summary = _readModelRepository.Value.Retrieve(evt.AggregateId);
            if (summary != null)
            {
                summary.Active = false;
                summary.End = evt.StopTime;
                _readModelRepository.Value.Update(summary);
            }
        }

        public void Handle(ReceivePingResponse cmd)
        {
            using (var repository = _writeRepositoryFactory())
            {
                PingAggregate ping = repository.GetById<PingAggregate>(cmd.AggregateId) ??
                                 new PingAggregate(new RegistrationEventRouter(), cmd.AggregateId);
                ping.ReceivePingResponse(cmd);

                repository.Save(ping, Guid.NewGuid());
                if (ping.IsActive())
                {
                    _bus.Value.Publish(new PongRequested
                    {
                        AggregateId = ping.Id,
                        RequestTime = DateTimeOffset.UtcNow
                    });
                }
            }
            
            
        }

        public void Handle(StartPing cmd)
        {
            using (var repository = _writeRepositoryFactory())
            {
                var ping = repository.GetById<PingAggregate>(cmd.AggregateId);
                ping.Start(cmd);

                repository.Save(ping, Guid.NewGuid());
                _bus.Value.Publish(new PongRequested
                {
                    AggregateId = ping.Id,
                    RequestTime = DateTimeOffset.UtcNow
                });
            }
        }

        public void Handle(StopPing cmd)
        {
            using (var repository = _writeRepositoryFactory())
            {
                PingAggregate ping = repository.GetById<PingAggregate>(cmd.AggregateId) ??
                                     new PingAggregate(new RegistrationEventRouter(), cmd.AggregateId);
                ping.Stop(cmd);

                repository.Save(ping, Guid.NewGuid());
            }
        }

        public void Handle(PongSent msg)
        {
            _bus.Value.Send(new ReceivePingResponse()
            {
                AggregateId = msg.PingId,
                ResponseTime = msg.SendTime
            });
        }
    }
}