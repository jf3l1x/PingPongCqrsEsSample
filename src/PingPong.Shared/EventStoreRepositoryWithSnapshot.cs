using System;
using System.Collections.Generic;
using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using NEventStore;

namespace PingPong.Shared
{
    public class EventStoreRepositoryWithSnapshot : EventStoreRepository
    {
        private readonly IStoreEvents _eventStore;

        public EventStoreRepositoryWithSnapshot(IStoreEvents eventStore, IConstructAggregates factory,
            IDetectConflicts conflictDetector) : base(eventStore, factory, conflictDetector)
        {
            _eventStore = eventStore;
        }

        public override void Save(IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
            IMemento snapshot = null;
            if (aggregate.Version%50 < aggregate.GetUncommittedEvents().Count)
            {
                snapshot = aggregate.GetSnapshot();
            }
            base.Save(aggregate, commitId, updateHeaders);
            if (snapshot != null)
            {
                _eventStore.Advanced.AddSnapshot(new Snapshot(aggregate.Id.ToString(),aggregate.Version,snapshot));
            }
            
        }
    }
}