﻿using System;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using Ping.Model;
using Ping.Model.Domain;

namespace Ping.Services.Default
{
    public class AggregateFactory : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
            return new PingAggregate(new RegistrationEventRouter(), id);
        }
    }
}