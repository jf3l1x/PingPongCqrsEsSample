using System;
using CommonDomain.Core;
using CommonDomain.Persistence;
using Ping.Messages.Commands;
using Ping.Messages.ExternalEvents;
using Ping.Model;
using PingPong.Shared;

namespace Ping.Handlers.Commands
{
    public class CmdHandler:IHandle<StartPing>,IHandle<StopPing>,IHandle<ReceivePingResponse>
    {
        private readonly IRepository _repository;
        private readonly IServiceBus _bus;

        public CmdHandler(IRepository repository,IServiceBus bus)
        {
            _repository = repository;
            _bus = bus;
        }

        public void Handle(StartPing cmd)
        {
            var ping=_repository.GetById<PingAggregate>(cmd.AggregateId) ??
                     new PingAggregate(new RegistrationEventRouter(), cmd.AggregateId);
            ping.Start(cmd);

            _repository.Save(ping,Guid.NewGuid());
            _bus.Publish(new PongRequested()
            {
                AggregateId = ping.Id,
                RequestTime = DateTimeOffset.UtcNow
            });
        }

        public void Handle(StopPing cmd)
        {
            var ping = _repository.GetById<PingAggregate>(cmd.AggregateId) ??
                    new PingAggregate(new RegistrationEventRouter(), cmd.AggregateId);
            ping.Stop(cmd);

            _repository.Save(ping, Guid.NewGuid());
            
        }

        public void Handle(ReceivePingResponse cmd)
        {
            var ping = _repository.GetById<PingAggregate>(cmd.AggregateId) ??
                  new PingAggregate(new RegistrationEventRouter(), cmd.AggregateId);
            ping.ReceivePingResponse(cmd);

            _repository.Save(ping, Guid.NewGuid());
            if (ping.IsActive())
            {
                _bus.Publish(new PongRequested()
                {
                    AggregateId = ping.Id,
                    RequestTime = DateTimeOffset.UtcNow
                });
            }
        }
    }
}