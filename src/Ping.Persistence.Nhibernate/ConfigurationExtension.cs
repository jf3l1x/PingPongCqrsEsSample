using Constant.Module.Interfaces.Configuration;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using Ping.Persistence.Nhibernate.Mappings;
using Ping.Shared.Model.Read;

namespace Ping.Persistence.Nhibernate
{
    public static class ConfigurationExtension
    {
        public static Configuration ConfigurePingPersistence(this Configuration configuration,IRegisterRepository container)
        {
            var mapper = new ModelMapper();
            mapper.AddMapping<PingSummaryMap>();
            configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
            container.RegisterReadFromRepository<PingSummary,NHibernateRepository<PingSummary>>();
            container.RegisterReadRepository<PingSummary, NHibernateRepository<PingSummary>>();
            container.RegisterWriteToRepository<PingSummary, NHibernateRepository<PingSummary>>();
            return configuration;
        }
    }
}