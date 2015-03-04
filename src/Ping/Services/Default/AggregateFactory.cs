using System;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using Ping.Model;
using Ping.Model.Domain;
using PingPong.Shared;

namespace Ping.Services.Default
{
    public class AggregateFactory : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
            var mySnapshot = snapshot as SnapShot;
            if (mySnapshot != null)
            {
                var data = mySnapshot.GetObject();
                return new PingAggregate(new RegistrationEventRouter(), id, (bool)data.Active, (int)data.Count, (int)data.CountLimit,
                    (DateTimeOffset)data.StartTime, (TimeSpan)data.TimeLimit, (int)data.TotalCount,mySnapshot.Version);
            }
            else
            {
                return new PingAggregate(new RegistrationEventRouter(), id);
            }
        }
    }
}