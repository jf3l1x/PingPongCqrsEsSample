using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ping.Model.Read;

namespace Ping.Persistence.NHibernate.Mappings
{
    public class PingSummaryMap : ClassMapping<PingSummary>
    {
        public PingSummaryMap()
        {
            Table("PingSummaries");

            Lazy(false);
            
            Id(x => x.Id, m => m.Generator(Generators.Assigned));
            
            Property(x => x.Start);
            Property(x => x.End, mapper => mapper.Column("[End]"));
            Property(x => x.PingsPerSecond);
            Property(x => x.TotalResponses);
            Property(x => x.Active);
        }
    }
}