using System.Linq;
using Constant.Module.Interfaces.Persistence.ReadModel;
using NHibernate;
using NHibernate.Linq;

namespace Ping.Persistence.Nhibernate
{
    public class NHibernateRepository<T> : IReadRepository<T>
    {
        private readonly IStatelessSession _session;

        public NHibernateRepository(IStatelessSession session)
        {
            _session = session;
        }

        public void Create(T obj)
        {
            using (var tx=_session.BeginTransaction())
            {
                _session.Insert(obj);
                tx.Commit();
            }
            
        }

        public T Retrieve(object id)
        {

            return _session.Get<T>(id);
            
        }

        public IQueryable<T> Query()
        {
            return _session.Query<T>();
        }

        public void Update(T obj)
        {
            using (var tx = _session.BeginTransaction())
            {
                _session.Update(obj);
                tx.Commit();
            }
        }


        public void Delete(object id)
        {
            using (var tx = _session.BeginTransaction())
            {
                var toDelete = _session.Get<T>(id);
                _session.Delete(toDelete);
                tx.Commit();
            }
           
        }
    }
}