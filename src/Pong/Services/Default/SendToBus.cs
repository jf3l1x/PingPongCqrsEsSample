using System;
using NEventStore;
using PingPong.Shared;

namespace Pong.Services.Default
{
    internal class SendToBus : IPipelineHook
    {
        private readonly Lazy<IServiceBus> _serviceBus;


        public SendToBus(Func<IServiceBus> serviceBusFactory)
        {
            _serviceBus = new Lazy<IServiceBus>(serviceBusFactory);
        }

        public void Dispose()
        {
        }

        public ICommit Select(ICommit committed)
        {
            return committed;
        }

        public bool PreCommit(CommitAttempt attempt)
        {
            return true;
        }

        public void PostCommit(ICommit committed)
        {
            foreach (EventMessage evt in committed.Events)
            {
                _serviceBus.Value.Send(evt.Body);
            }
        }

        public void OnPurge(string bucketId)
        {
        }

        public void OnDeleteStream(string bucketId, string streamId)
        {
        }
    }
}