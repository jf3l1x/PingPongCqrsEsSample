namespace Pong
{
    
    public interface IHandle<T>
    {
        void Handle(T msg);
    }
}