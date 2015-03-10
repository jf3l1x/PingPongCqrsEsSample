using System;
using System.Threading.Tasks;
using CommonDomain.Core;
using CommonDomain.Persistence;
using Constant.Module.Interfaces.Bus;
using Constant.Module.Interfaces.Persistence.ReadModel;
using Ping.Shared.Messages.Commands;
using Ping.Shared.Messages.Events;
using Ping.Shared.Messages.ExternalEvents;
using Ping.Shared.Model.Domain;
using Ping.Shared.Model.Read;


namespace Ping.Worker.Handlers
{
    public class DefaultHandler : IHandleMessages<StartPing>, IHandleMessages<StopPing>, IHandleMessages<ReceivePingResponse>,
        IHandleMessages<PingResponseReceived>, IHandleMessages<PingStarted>, IHandleMessages<PingStopped>, IHandleMessages<PongSent>
    {
        private readonly Lazy<IServiceBus> _bus;
        private readonly Lazy<IReadRepository<PingSummary>> _readModelRepository;
        private readonly Func<IRepository> _writeRepositoryFactory;


        public DefaultHandler(Func<IRepository> writeRepositoryFactory, Func<IServiceBus> busFactory,
            Func<IReadRepository<PingSummary>> readRepositoryFactory)
        {
            _writeRepositoryFactory = writeRepositoryFactory;
            _bus = new Lazy<IServiceBus>(busFactory);
            _readModelRepository = new Lazy<IReadRepository<PingSummary>>(readRepositoryFactory);
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

        public Task HandleAsync(PingResponseReceived message)
        {
            return Task.Run(() => Handle(message));
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

        public Task HandleAsync(PingStarted message)
        {
            return Task.Run(() => Handle(message));
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

        public Task HandleAsync(PingStopped message)
        {
            return Task.Run(() => Handle(message));
        }

        public void Handle(PongSent msg)
        {
            _bus.Value.Send(new ReceivePingResponse
            {
                AggregateId = msg.PingId,
                ResponseTime = msg.SendTime
            });
        }

        public Task HandleAsync(PongSent message)
        {
            return Task.Run(() => Handle(message));
        }

        public void Handle(ReceivePingResponse cmd)
        {
            using (IRepository repository = _writeRepositoryFactory())
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

        public Task HandleAsync(ReceivePingResponse message)
        {
            return Task.Run(() => Handle(message));
        }

        public void Handle(StartPing cmd)
        {
            using (IRepository repository = _writeRepositoryFactory())
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

        public Task HandleAsync(StartPing message)
        {
            return Task.Run(() => Handle(message));
        }

        public void Handle(StopPing cmd)
        {
            using (IRepository repository = _writeRepositoryFactory())
            {
                PingAggregate ping = repository.GetById<PingAggregate>(cmd.AggregateId) ??
                                     new PingAggregate(new RegistrationEventRouter(), cmd.AggregateId);
                ping.Stop(cmd);

                repository.Save(ping, Guid.NewGuid());
            }
        }

        public Task HandleAsync(StopPing message)
        {
            return Task.Run(() => Handle(message));
        }
    }
}