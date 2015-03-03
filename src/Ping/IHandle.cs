namespace Ping
{
    
    public interface IHandle<T>
    {
        void Handle(T msg);
    }
}