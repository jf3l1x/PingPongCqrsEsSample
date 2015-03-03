using System;
using System.Collections.Generic;
using NEventStore;
using PingPong.Shared;

namespace Ping.Services.Default
{
    internal class SendToBus : IPipelineHook
    {
        private readonly IServiceBus _serviceBus;
        
        

        public SendToBus(IServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
            
            
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
            foreach (var evt in committed.Events)
            {
                _serviceBus.Send(evt.Body);
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