using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Wrappers;
using Ping.Model.Read;
using PingPong.Shared;

namespace Ping.Persistence.MongoDB
{
    public class PingSummaryRepositoryMongoDB : IReadModelRepository<PingSummary>
    {
        private readonly MongoDatabase _database;

        public PingSummaryRepositoryMongoDB(MongoDatabase database)
        {
            _database = database;
        }

        public void Create(PingSummary obj)
        {
            MongoCollection<PingSummary> collection = GetCollection();
            collection.Save(obj);
        }

        public PingSummary Retrieve(object id)
        {
            IMongoQuery query = Query<PingSummary>.EQ(e => e.Id, id);
            return GetCollection().FindOneAs<PingSummary>(query);
        }

        public IQueryable<PingSummary> Query()
        {
            return ((MongoCollection) GetCollection()).AsQueryable<PingSummary>();
        }

        public void Update(PingSummary obj)
        {
            IMongoQuery query = Query<PingSummary>.EQ(e => e.Id, obj.Id);

            GetCollection().FindAndModify(new FindAndModifyArgs
            {
                Query = query,
                Update = new UpdateWrapper(typeof (PingSummary), obj)
            });
        }

        public void Delete(PingSummary obj)
        {
            IMongoQuery query = Query<PingSummary>.EQ(e => e.Id, obj.Id);
            GetCollection().Remove(query);
        }

        private MongoCollection<PingSummary> GetCollection()
        {
            return _database.GetCollection<PingSummary>("PingSummaries");
        }
    }
}