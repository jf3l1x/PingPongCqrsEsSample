using System.Linq;
using NHibernate;
using NHibernate.Linq;
using PingPong.Shared;
using Pong.Model.Read;

namespace Pong.Persistence.NHibernate
{
    public class PongSummaryRepository : IReadModelRepository<PongSummary>
    {
        private readonly IStatelessSession _session;

        public PongSummaryRepository(IStatelessSession session)
        {
            _session = session;
        }

        public void Create(PongSummary obj)
        {
            _session.Update(obj);
        }

        public PongSummary Retrieve(object id)
        {
            return _session.Get<PongSummary>(id);
        }

        public IQueryable<PongSummary> Query()
        {
            return _session.Query<PongSummary>();
        }

        public void Update(PongSummary obj)
        {
            _session.Update(obj);
        }

        public void Delete(PongSummary obj)
        {
            var toDelete = _session.Get<PongSummary>(obj.Id);
            _session.Delete(toDelete);
        }
    }
}