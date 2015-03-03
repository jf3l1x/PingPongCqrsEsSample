using System;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using Pong.Model.Domain;

namespace Pong.Services.Default
{
    public class AggregateFactory : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
            return new PongAggregate(new RegistrationEventRouter(), id);
        }
    }
}
