using System.Data.Entity;
using System.Linq;
using PingPong.Shared;
using Pong.Model.Read;

namespace Pong.Persistence.EntityFramework
{
    
    internal class PongSummaryContext : DbContext, IReadModelRepository<PongSummary>
    {

        public PongSummaryContext(IGiveTenantConfiguration tenantConfiguration) : base(tenantConfiguration.GetConnectionString())
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<PongSummaryContext>());
        }
        
        public DbSet<PongSummary> Pongs { get; set; }

        public void Create(PongSummary obj)
        {
            Pongs.Add(obj);
            SaveChanges();
        }

        public PongSummary Retrieve(object id)
        {
            return Pongs.Find(id);
        }

        public IQueryable<PongSummary> Query()
        {
            return Pongs.AsQueryable();
        }

        public void Update(PongSummary obj)
        {
            Pongs.Attach(obj);
            Entry(obj).State = EntityState.Modified;
            SaveChanges();
        }

        public void Delete(PongSummary obj)
        {
            PongSummary entity = Pongs.Find(obj.Id);
            Pongs.Remove(entity);
            SaveChanges();
        }
    }
}
