using System;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using PingPong.Shared;
using Pong.Model.Domain;

namespace Pong.Services.Default
{
    public class AggregateFactory : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
            var mySnapshot = snapshot as SnapShot;
            if (mySnapshot != null)
            {

                return new PongAggregate(new RegistrationEventRouter(), id,(int)mySnapshot.GetObject().Count,mySnapshot.Version);
            }
            else
            {
                return new PongAggregate(new RegistrationEventRouter(), id);    
            }
            
        }
    }
}
