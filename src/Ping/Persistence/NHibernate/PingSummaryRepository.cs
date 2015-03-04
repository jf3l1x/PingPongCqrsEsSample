using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Ping.Model.Read;
using PingPong.Shared;

namespace Ping.Persistence.NHibernate
{
    public class PingSummaryRepository : IReadModelRepository<PingSummary>
    {
        private readonly IStatelessSession _session;

        public PingSummaryRepository(IStatelessSession session)
        {
            _session = session;
        }

        public void Create(PingSummary obj)
        {
            _session.Update(obj);
        }

        public PingSummary Retrieve(object id)
        {
            return _session.Get<PingSummary>(id);
        }

        public IQueryable<PingSummary> Query()
        {
            return _session.Query<PingSummary>();
        }

        public void Update(PingSummary obj)
        {
            _session.Update(obj);
        }

        public void Delete(PingSummary obj)
        {
            var toDelete = _session.Get<PingSummary>(obj.Id);
            _session.Delete(toDelete);
        }
    }
}