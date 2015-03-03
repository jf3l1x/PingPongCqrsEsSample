namespace Ping.Handlers
{
    public interface ICreateHandlers
    {
        IHandle<T> Create<T>();
    }
}