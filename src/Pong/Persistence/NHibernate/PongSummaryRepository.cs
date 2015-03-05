using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using PingPong.Shared;
using Pong.Model.Read;

namespace Pong.Persistence.NHibernate
{
    public class PongSummaryRepository : IReadModelRepository<PongSummary>
    {
        private readonly Func<IStatelessSession> _sessionFactory;

        public PongSummaryRepository(Func<IStatelessSession> sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void Create(PongSummary obj)
        {
            var session = _sessionFactory();

            using (var transaction=session.BeginTransaction())
            {
                session.Insert(obj);    
                transaction.Commit();
            }
        }

        public PongSummary Retrieve(object id)
        {
            return _sessionFactory().Get<PongSummary>(id);
        }

        public IQueryable<PongSummary> Query()
        {
            return _sessionFactory().Query<PongSummary>();
        }

        public void Update(PongSummary obj)
        {
            var session = _sessionFactory();
            
            using (var transaction = session.BeginTransaction())
            {
                session.Update(obj);
                transaction.Commit();
            }
        }

        public void Delete(PongSummary obj)
        {
            var session = _sessionFactory();

            using (var transaction = session.BeginTransaction())
            {
                var toDelete = session.Get<PongSummary>(obj.Id);
                session.Delete(toDelete);
                transaction.Commit();
            }
        }
    }
}