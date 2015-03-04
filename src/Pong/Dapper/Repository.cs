using System.Data;
using System.Linq;
using Dapper;
using NEventStore.Persistence.Sql;
using PingPong.Shared;
using Pong.Model.Read;

namespace Pong.Dapper
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
            using (IDbConnection connection = _factory.Open())
            {
                connection.Execute(
                    "insert into pongsummaries (Id,PingId,Count,RequestTime) values (@Id,@PingId,@Count,@RequestTime)",
                    obj);
            }
        }

        public PongSummary Retrieve(object id)
        {
            using (IDbConnection connection = _factory.Open())
            {
                return connection.Query<PongSummary>(
                    "select Id,PingId,Count,RequestTime from pongsummaries where Id=@Id",
                    new {Id = id}).FirstOrDefault();
            }
        }

        public IQueryable<PongSummary> Query()
        {
            using (IDbConnection connection = _factory.Open())
            {
                //Not Ling Support :(
                return
                    connection.Query<PongSummary>(
                        "select Id,PingId,Count,RequestTime from pongsummaries").AsQueryable();
            }
        }

        public void Update(PongSummary obj)
        {
            using (IDbConnection connection = _factory.Open())
            {
                connection.Execute(
                    "update pongsummaries Set Count=@Count,RequestTime=@RequestTime where Id=@Id",
                    obj);
            }
        }

        public void Delete(PongSummary obj)
        {
            using (IDbConnection connection = _factory.Open())
            {
                connection.Execute("delete pongsummaries where Id=@Id", obj);
            }
        }
    }
}