namespace Ping.Handlers
{
    public interface ICreateCmdHandle
    {
        IHandle<T> Create<T>();
    }
}