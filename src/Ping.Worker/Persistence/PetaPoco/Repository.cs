using System.Linq;
using NEventStore.Persistence.Sql;
using Ping.Shared.Model.Read;
using Ping.Worker.Persistence.PetaPoco.PetaPoco;
using PingPong.Shared;

namespace Ping.Worker.Persistence.PetaPoco
{
    internal class Repository : IReadModelRepository<PingSummary>
    {
        private readonly IConnectionFactory _factory;

        public Repository(IConnectionFactory factory)
        {
            _factory = factory;
        }

        public void Create(PingSummary obj)
        {
            using (var db = new Database(_factory.Open()))
            {
                db.Insert("pingsummaries", "Id",false, obj);
            }
        }

        public PingSummary Retrieve(object id)
        {
            using (var db = new Database(_factory.Open()))
            {
                return db.SingleOrDefault<PingSummary>("SELECT * FROM pingsummaries WHERE Id=@0", id);
            }
        }

        public IQueryable<PingSummary> Query()
        {
            using (var db = new Database(_factory.Open()))
            {
                return db.Query<PingSummary>("SELECT * FROM pingsummaries").AsQueryable();
            }
        }

        public void Update(PingSummary obj)
        {
            using (var db = new Database(_factory.Open()))
            {
                db.Update("pingsummaries", "Id", obj);
            }
        }

        public void Delete(PingSummary obj)
        {
            using (var db = new Database(_factory.Open()))
            {
                db.Delete("pingsummaries", "Id", obj);
            }
        }
    }
}