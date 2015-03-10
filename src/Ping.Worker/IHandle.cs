namespace Ping.Worker
{
    
    public interface IHandle<T>
    {
        void Handle(T msg);
    }
}