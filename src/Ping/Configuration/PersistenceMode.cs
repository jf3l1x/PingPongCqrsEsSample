namespace Ping.Configuration
{
    public enum PersistenceMode
    {
        EntityFramework,
        NHibernate,
        PetaPoco,
        Dapper,
        GetEventStore
    }
}