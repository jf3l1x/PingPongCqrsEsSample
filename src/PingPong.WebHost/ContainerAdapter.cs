using Constant.Module.Interfaces.Configuration;
using Constant.Module.Interfaces.Persistence.ReadModel;
using LightInject;

namespace PingPong.WebHost
{
    internal class ContainerAdapter : IRegisterRepository
    {
        private readonly ServiceContainer _container;

        public ContainerAdapter(ServiceContainer container)
        {
            _container = container;
        }

        public void RegisterReadRepository<TEntity, TImplementor>() where TImplementor : IReadRepository<TEntity>
        {
            _container.Register<IReadRepository<TEntity>, TImplementor>();
        }

        public void RegisterReadFromRepository<TEntity, TImplementor>()
            where TImplementor : IReadFromRepository<TEntity>
        {
            _container.Register<IReadFromRepository<TEntity>, TImplementor>();
        }

        public void RegisterWriteToRepository<TEntity, TImplementor>() where TImplementor : IWriteToRepository<TEntity>
        {
            _container.Register<IWriteToRepository<TEntity>, TImplementor>();
        }
    }
}