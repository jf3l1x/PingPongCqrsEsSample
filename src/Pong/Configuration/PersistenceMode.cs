namespace Pong.Configuration
{
    public enum PersistenceMode
    {
        EntityFramework,
        NHibernate,
        PetaPoco,
        Dapper,
        GetEventStore,
        MongoDB
    }
}