namespace Ping.Worker
{
    public interface ICreateHandlers
    {
        IHandle<T> Create<T>();
    }
}