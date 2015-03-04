using System;
using System.Linq;
using CommonDomain.Core;
using CommonDomain.Persistence;
using PingPong.Shared;
using Pong.Messages.Commands;
using Pong.Messages.Events;
using Pong.Messages.ExternalEvents;
using Pong.Model.Domain;
using Pong.Model.Read;

namespace Pong.Handlers.Commands
{
    internal class DefaultHandler : IHandle<GeneratePong>, IHandle<PongGenerated>, IHandle<PongRequested>
    {

        private readonly Lazy<IServiceBus> _bus;
        private readonly Lazy<IReadModelRepository<PongSummary>> _readModelRepository;
        private readonly Lazy<IRepository> _writeModelRepository;

        public DefaultHandler(Func<IRepository> writeRepositoryFactory, Func<IServiceBus> busFactory,
            Func<IReadModelRepository<PongSummary>> readRepositoryFactory)
        {
            _writeModelRepository = new Lazy<IRepository>(writeRepositoryFactory);
            _bus = new Lazy<IServiceBus>(busFactory);
            _readModelRepository = new Lazy<IReadModelRepository<PongSummary>>(readRepositoryFactory);
        }

        public void Handle(GeneratePong cmd)
        {
            PongAggregate pong;
            var pongReadModel=_readModelRepository.Value.Query().FirstOrDefault(p => p.PingId == cmd.PingId);
            if (pongReadModel!=null)
            {
                pong=_writeModelRepository.Value.GetById<PongAggregate>(pongReadModel.Id);
            }
            else
            {
                pong = new PongAggregate(new RegistrationEventRouter(), cmd.AggregateId);    
            }
            
            pong.Generate(cmd);

            _writeModelRepository.Value.Save(pong, Guid.NewGuid());

            _bus.Value.Publish(new PongSent
            {
                AggregateId = pong.Id,
                PingId = cmd.PingId,
                SendTime = DateTimeOffset.UtcNow
            });
        }

        public void Handle(PongGenerated evt)
        {
            PongSummary summary = _readModelRepository.Value.Retrieve(evt.AggregateId);
            if (summary == null)
            {
                summary = new PongSummary
                {
                    Id = evt.AggregateId,
                    PingId = evt.PingId,
                    Count = 0,
                    RequestTime = evt.RequestTime
                };
                _readModelRepository.Value.Create(summary);
            }
            else
            {
                summary.Count++;
                _readModelRepository.Value.Update(summary);
            }
        }

        public void Handle(PongRequested msg)
        {
            _bus.Value.Send(new GeneratePong
            {
                PingId = msg.AggregateId,
                RequestTime = msg.RequestTime
            });
        }
    }
}
