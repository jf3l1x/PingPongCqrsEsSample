namespace Ping.Handlers
{
    public interface ICreateHandle
    {
        IHandle<T> Create<T>();
    }
}