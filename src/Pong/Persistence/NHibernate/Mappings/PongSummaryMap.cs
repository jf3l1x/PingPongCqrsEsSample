using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Pong.Model.Read;

namespace Pong.Persistence.NHibernate.Mappings
{
    public class PongSummaryMap : ClassMapping<PongSummary>
    {
        public PongSummaryMap()
        {
            Table("PongSummaries");

            Lazy(false);
            
            Id(x => x.Id, m => m.Generator(Generators.Assigned));
           
            Property(x => x.Count);
            Property(x => x.PingId);
            Property(x => x.RequestTime);
        }
    }
}