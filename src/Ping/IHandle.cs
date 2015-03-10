namespace Ping.Web
{
    
    public interface IHandle<T>
    {
        void Handle(T msg);
    }
}