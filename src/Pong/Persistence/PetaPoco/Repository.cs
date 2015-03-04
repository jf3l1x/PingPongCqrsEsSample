using System;
using System.Linq;
using NEventStore.Persistence.Sql;
using PingPong.Shared;
using Pong.Model.Read;
using Pong.Persistence.PetaPoco.PetaPoco;

namespace Pong.Persistence.PetaPoco
{
    internal class Repository : IReadModelRepository<PongSummary>
    {
        private readonly IConnectionFactory _factory;

        public Repository(IConnectionFactory factory)
        {
            _factory = factory;
        }

        public void Create(PongSummary obj)
        {
            using (var db = new Database(_factory.Open()))
            {
                db.Insert("pongsummaries", "Id", false, obj);
            }
        }

        public PongSummary Retrieve(object id)
        {
            using (var db = new Database(_factory.Open()))
            {
                return db.SingleOrDefault<PongSummary>("SELECT * FROM pongsummaries WHERE Id=@0", id);
            }
        }

        public IQueryable<PongSummary> Query()
        {
            using (var db = new Database(_factory.Open()))
            {
                return db.Query<PongSummary>("SELECT * FROM pongsummaries").AsQueryable();
            }
        }

        public void Update(PongSummary obj)
        {
            using (var db = new Database(_factory.Open()))
            {
                db.Update("pongsummaries", "Id", obj);
            }
        }

        public void Delete(PongSummary obj)
        {
            using (var db = new Database(_factory.Open()))
            {
                db.Delete("pongsummaries", "Id", obj);
            }
        }
    }
}