using System;
using Constant.Module.Interfaces.Configuration;

namespace PingPong.Worker
{
    [Serializable]
    public class WorkerModuleContainer : IWorkerModuleContainer
    {
        private readonly IGiveTenantConfiguration _configurator;


        public WorkerModuleContainer(IGiveTenantConfiguration configurator)
        {
            _configurator = configurator;
        }

        public IGiveTenantConfiguration CreateTenantConfiguration()
        {
            return _configurator;
        }
    }
}