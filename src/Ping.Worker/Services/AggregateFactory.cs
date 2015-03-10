using System;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using Ping.Shared.Model.Domain;

namespace Ping.Worker.Services
{
    public class AggregateFactory : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
           
            return new PingAggregate(new RegistrationEventRouter(), id);
           
        }
    }
}