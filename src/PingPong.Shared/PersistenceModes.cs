namespace PingPong.Shared
{
    public enum ReadPersistenceMode
    {
        EntityFramework,
        NHibernate,
        PetaPoco,
        Dapper,
        MongoDB

    }
    public enum WritePersistenceMode
    {
        SqlServer,
        GetEventStore,
        MongoDB

    }
}