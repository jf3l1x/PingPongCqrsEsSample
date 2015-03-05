using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Ping.Model.Read;
using PingPong.Shared;

namespace Ping.Persistence.NHibernate
{
    public class PingSummaryRepository : IReadModelRepository<PingSummary>
    {
        private readonly Func<IStatelessSession> _sessionFactory;


        public PingSummaryRepository(Func<IStatelessSession> sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void Create(PingSummary obj)
        {
            var session = _sessionFactory();

            using (var transaction=session.BeginTransaction())
            {
                session.Insert(obj);    
                transaction.Commit();
            }
        }

        public PingSummary Retrieve(object id)
        {
            return _sessionFactory().Get<PingSummary>(id);
        }

        public IQueryable<PingSummary> Query()
        {
            return _sessionFactory().Query<PingSummary>();
        }

        public void Update(PingSummary obj)
        {
            var session = _sessionFactory();
            
            using (var transaction = session.BeginTransaction())
            {
                session.Update(obj);
                transaction.Commit();
            }
        }

        public void Delete(PingSummary obj)
        {
            var session = _sessionFactory();

            using (var transaction = session.BeginTransaction())
            {
                var toDelete = session.Get<PingSummary>(obj.Id);
                session.Delete(toDelete);
                transaction.Commit();
            }
        }
    }
}