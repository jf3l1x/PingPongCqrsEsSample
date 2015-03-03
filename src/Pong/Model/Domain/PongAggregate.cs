using System;
using CommonDomain;
using CommonDomain.Core;
using Pong.Messages.Commands;
using Pong.Messages.Events;

namespace Pong.Model.Domain
{
    public class PongAggregate : AggregateBase
    {
        private int _count;
        private Guid _pingId;

        public PongAggregate(IRouteEvents handler, Guid id) : base(handler)
        {
            Id = id;
            handler.Register<PongGenerated>(Handle);
        }

        public void Generate(GeneratePong cmd)
        {
            RaiseEvent(new PongGenerated
            {
                AggregateId = Id,
                PingId = cmd.PingId
            });
        }

        private void Handle(PongGenerated evt)
        {
            _pingId = evt.PingId;
            _count++;
        }

    }
}
