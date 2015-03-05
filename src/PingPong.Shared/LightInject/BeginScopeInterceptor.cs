using LightInject;
using LightInject.Interception;

namespace PingPong.Shared.LightInject
{
    public class BeginScopeInterceptor : IInterceptor
    {
        private readonly ServiceContainer _container;

        public BeginScopeInterceptor(ServiceContainer container)
        {
            _container = container;
        }

        public object Invoke(IInvocationInfo invocationInfo)
        {
            using (_container.BeginScope())
            {
                return invocationInfo.Proceed();   
            }
        }
    }
}
