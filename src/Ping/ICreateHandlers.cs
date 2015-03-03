namespace Ping
{
    public interface ICreateHandlers
    {
        IHandle<T> Create<T>();
    }
}