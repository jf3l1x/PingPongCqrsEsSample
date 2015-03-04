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
            using (var session = _sessionFactory())
            {
                using (var transaction=session.BeginTransaction())
                {
                    session.Insert(obj);    
                    transaction.Commit();
                }
                
            }
            
        }

        public PingSummary Retrieve(object id)
        {

            using (var session = _sessionFactory())
            {
                return session.Get<PingSummary>(id);
            }
            
        }

        public IQueryable<PingSummary> Query()
        {
            //using (var session = _sessionFactory())
            //{
            //    return session.Query<PingSummary>().ToList().AsQueryable();
            //}

            // TODO Need close session using open session in view strategy because OData need to change queryable returned by this repository before complete http response.
            return _sessionFactory().Query<PingSummary>();
        }

        public void Update(PingSummary obj)
        {
            using (var session = _sessionFactory())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(obj);
                    transaction.Commit();
                }
            }
        }

        public void Delete(PingSummary obj)
        {
            using (var session = _sessionFactory())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var toDelete = session.Get<PingSummary>(obj.Id);
                    session.Delete(toDelete);
                    transaction.Commit();
                }

            }
            
            
            
        }
    }
}