using PingPong.Shared;

namespace Pong
{
    public interface ICreateHandlers
    {
        IHandle<T> Create<T>();
    }
}