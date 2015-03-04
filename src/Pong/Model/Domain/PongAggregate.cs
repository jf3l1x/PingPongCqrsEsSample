using System;
using CommonDomain;
using CommonDomain.Core;
using PingPong.Shared;
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
        public PongAggregate(IRouteEvents handler, Guid id,int count,int version)
            : base(handler)
        {
            Id = id;
            Version = version;
            _count = count;
            handler.Register<PongGenerated>(Handle);
        }
        public void Generate(GeneratePong cmd)
        {
            RaiseEvent(new PongGenerated
            {
                AggregateId = Id,
                PingId = cmd.PingId,
                RequestTime = cmd.RequestTime
            });
        }

        private void Handle(PongGenerated evt)
        {
            _pingId = evt.PingId;
            _count++;
        }

        protected override IMemento GetSnapshot()
        {
            return new SnapShot(new { Count=_count,PingId=_pingId})
            {
                Id = Id,
                Version = Version
            };
        }
        
    }
}