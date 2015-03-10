using System.Data;
using System.Linq;
using Constant.Module.Interfaces.Persistence.ReadModel;
using Dapper;
using NEventStore.Persistence.Sql;
using Ping.Shared.Model.Read;

namespace Ping.Worker.Persistence.Dapper
{
    internal class Repository :IReadRepository<PingSummary>
    {
        private readonly IConnectionFactory _factory;

        public Repository(IConnectionFactory factory)
        {
            _factory = factory;
        }

        public PingSummary Retrieve(object id)
        {
            using (IDbConnection connection = _factory.Open())
            {
                return connection.Query<PingSummary>(
                    "select Id ,TotalResponses,Active,Start,[End],PingsPerSecond from pingsummaries where Id=@Id",
                    new {Id = id}).FirstOrDefault();
            }
        }

        public IQueryable<PingSummary> Query()
        {
            using (IDbConnection connection = _factory.Open())
            {
                //Not Ling Support :(
                return
                    connection.Query<PingSummary>(
                        "select Id ,TotalResponses,Active,Start,[End],PingsPerSecond from pingsummaries").AsQueryable();
            }
        }

        public void Create(PingSummary obj)
        {
            using (IDbConnection connection = _factory.Open())
            {
                connection.Execute(
                    "insert into pingsummaries (Id,TotalResponses,Active,Start,[End],PingsPerSecond) values (@Id,@TotalResponses,@Active,@Start,@End,@PingsPerSecond)",
                    obj);
            }
        }

        public void Update(PingSummary obj)
        {
            using (IDbConnection connection = _factory.Open())
            {
                connection.Execute(
                    "update pingsummaries Set TotalResponses=@TotalResponses,Active=@Active,Start=@Start,[End]=@End,PingsPerSecond=@PingsPerSecond where Id=@Id",
                    obj);
            }
        }

        public void Delete(PingSummary obj)
        {
            using (IDbConnection connection = _factory.Open())
            {
                connection.Execute("delete pingsummaries where Id=@Id", obj);
            }
        }
    }
}