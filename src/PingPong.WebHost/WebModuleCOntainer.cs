﻿using System;
using System.Collections.Generic;
using Constant.Hosting.Rebus;
using Constant.Module.Interfaces.Bus;
using Constant.Module.Interfaces.Configuration;
using Constant.Module.Interfaces.Persistence.ReadModel;
using Rebus.RabbitMQ;

namespace PingPong.WebHost
{
    public class WebModuleContainer : IWebModuleContainer
    {
        public ISendMessages CreateBus(IActivateHandlers activator, IEnumerable<IMutateMessages> mutators,
            IRouteMessages router, IEnumerable<IResolveTypeName> resolvers)
        {
            var bus = new RebusAdapter();
            bus.RegisterActivatorFactory(() => activator);
            bus.RegisterMutators(mutators);
            bus.SetMessageRouter(router);
            bus.GetConfigurator().Transport(transport =>
            {
                var options = transport.UseRabbitMqInOneWayMode("amqp://jf3l1x:password@localhost:5672/testes")
                    .ManageSubscriptions()
                    .UseExchange("Rebus");
                foreach (var resolver in resolvers)
                {
                    var r = resolver;
                    options.AddEventNameResolver(r.Resolve);
                }
            });
                                    
            bus.Start();
            return bus;

        }

        public IReadFromRepository<T> CreateRepository<T>()
        {
            throw new NotImplementedException();
        }

        public IGiveTenantConfiguration CreateTenantConfiguration()
        {
            throw new NotImplementedException();
        }
    }
}