using System.Data.Entity;
using System.Linq;
using Ping.Model.Read;
using PingPong.Shared;

namespace Ping.Persistence.EntityFramework
{
    
    internal class PingSummaryContext : DbContext, IReadModelRepository<PingSummary>
    {

        public PingSummaryContext(IGiveTenantConfiguration tenantConfiguration):base(tenantConfiguration.GetConnectionString())
        {
            
        }

        public DbSet<PingSummary> Pings { get; set; }

        public void Create(PingSummary obj)
        {
            Pings.Add(obj);
            SaveChanges();
        }

        public PingSummary Retrieve(object id)
        {
            return Pings.Find(id);
        }

        public IQueryable<PingSummary> Query()
        {
            return Pings.AsQueryable();
        }

        public void Update(PingSummary obj)
        {
            Pings.Attach(obj);
            Entry(obj).State = EntityState.Modified;
            SaveChanges();
        }

        public void Delete(PingSummary obj)
        {
            PingSummary entity = Pings.Find(obj.Id);
            Pings.Remove(entity);
            SaveChanges();
        }
    }
}