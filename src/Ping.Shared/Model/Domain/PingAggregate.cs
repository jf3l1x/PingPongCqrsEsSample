using System;
using CommonDomain;
using CommonDomain.Core;
using Ping.Shared.Messages.Commands;
using Ping.Shared.Messages.Events;
using PingPong.Shared;

namespace Ping.Shared.Model.Domain
{
    public class PingAggregate : AggregateBase
    {
        private bool _active;
        private int _count;
        private int _countLimit;
        private DateTimeOffset _startTime;
        private TimeSpan _timeLimit;
        private int _totalCount;

        public PingAggregate(IRouteEvents handler,Guid id) : base(handler)
        {
            Id = id;
            RegisterHandlers(handler);
        }
        public PingAggregate(IRouteEvents handler, Guid id,bool active,int count,int countLimit,DateTimeOffset startTime,TimeSpan timelimit,int totalCount,int version)
            : base(handler)
        {
            Id = id;
            _active = active;
            _count = count;
            _countLimit = countLimit;
            _startTime = startTime;
            _timeLimit = timelimit;
            _totalCount = totalCount;
            Version = version;
            RegisterHandlers(handler);
        }

        private void RegisterHandlers(IRouteEvents handler)
        {
            handler.Register<PingResponseReceived>(Handle);
            handler.Register<PingStopped>(Handle);
            handler.Register<PingStarted>(Handle);
        }

        public void Start(StartPing cmd)
        {
            RaiseEvent(new PingStarted
            {
                AggregateId = cmd.AggregateId,
                CountLimit = cmd.CountLimit,
                StartTime = DateTimeOffset.UtcNow,
                TimeLimit = cmd.TimeLimit
            });
        }

        public bool IsActive()
        {
            return _active;
        }
        public void Stop(StopPing cmd)
        {
            RaiseEvent(new PingStopped
            {
                AggregateId = cmd.AggregateId,
                StopTime = DateTimeOffset.UtcNow
            });
        }

        public void ReceivePingResponse(ReceivePingResponse cmd)
        {
            RaiseEvent(new PingResponseReceived
            {
                AggregateId = cmd.AggregateId,
                ResponseTime = cmd.ResponseTime,
                ReceiveTime = DateTimeOffset.UtcNow
            });
            if (_countLimit > 0 && _countLimit < _count)
            {
                RaiseEvent(new PingStopped
                {
                    AggregateId = cmd.AggregateId,
                    StopTime = DateTimeOffset.UtcNow
                });
            }
            else
            {
                if (_timeLimit != TimeSpan.Zero && DateTimeOffset.UtcNow.Subtract(_startTime) > _timeLimit)
                {
                    RaiseEvent(new PingStopped
                    {
                        AggregateId = cmd.AggregateId,
                        StopTime = DateTimeOffset.UtcNow
                    });
                }
            }
        }

        #region Handles

        private void Handle(PingStarted obj)
        {
            _active = true;
            _startTime = obj.StartTime;
            _timeLimit = obj.TimeLimit;
            _countLimit = obj.CountLimit;
            _count = 0;
        }

        private void Handle(PingResponseReceived obj)
        {
            _count++;
            _totalCount++;
        }

        private void Handle(PingStopped obj)
        {
            _active = false;
        }

        protected override IMemento GetSnapshot()
        {
            return new SnapShot(new{Active=_active,Count=_count,CountLimit=_countLimit,TimeLimit=_timeLimit,StartTime=_startTime,TotalCount=_totalCount})
            {
                Id = Id,
                Version = Version
            };
        }

        #endregion
    }
}