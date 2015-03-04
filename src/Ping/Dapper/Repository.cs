using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using NEventStore.Persistence.Sql;
using Ping.Model.Read;
using PingPong.Shared;

namespace Ping.Dapper
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
            using (var connection = _factory.Open())
            {
                connection.Execute(
                    "insert into pingsummaries (Id,TotalResponses,Active,Start,[End],PingsPerSecond) values (@Id,@TotalResponses,@Active,@Start,@End,@PingsPerSecond)",
                    obj);    
            }
            
        }

        public PingSummary Retrieve(object id)
        {
            using (var connection = _factory.Open())
            {
                return connection.Query<PingSummary>(
                    "select Id ,TotalResponses,Active,Start,[End],PingsPerSecond from pingsummaries where Id=@Id",
                    new {Id = id}).FirstOrDefault();
            }
        }

        public IQueryable<PingSummary> Query()
        {
            using (var connection = _factory.Open())
            {
                //Not Ling Support :(
                return
                    connection.Query<PingSummary>(
                        "select Id ,TotalResponses,Active,Start,[End],PingsPerSecond from pingsummaries").AsQueryable();
            }
        }

        public void Update(PingSummary obj)
        {
            using (var connection = _factory.Open())
            {
                connection.Execute(
              "update pingsummaries Set TotalResponses=@TotalResponses,Active=@Active,Start=@Start,[End]=@End,PingsPerSecond=@PingsPerSecond where Id=@Id",
              obj);
            }
            
        }

        public void Delete(PingSummary obj)
        {
            using (var connection = _factory.Open())
            {
                connection.Execute("delete pingsummaries where Id=@Id", obj);
            }
        }
        
        
    }
}