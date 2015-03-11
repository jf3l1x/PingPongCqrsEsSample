using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ping.Shared.Model.Read;

namespace Ping.Persistence.Nhibernate.Mappings
{
    public class PingSummaryMap : ClassMapping<PingSummary>
    {
        public PingSummaryMap()
        {
            Table("pingsummaries");

            Lazy(false);
            
            Id(x => x.Id, m => m.Generator(Generators.Assigned));
           
            Property(x => x.Active);
            Property(x => x.End,map=>map.Column("[End]"));
            Property(x => x.PingsPerSecond);
            Property(x => x.Start);
            Property(x => x.TotalResponses);
        }
    }
}