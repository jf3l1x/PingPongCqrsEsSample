using Ping.Messages.Commands;

namespace Ping.Handlers
{
    public interface IHandle<T>
    {
        void Handle(T cmd);
    }
}