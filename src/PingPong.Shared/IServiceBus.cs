namespace PingPong.Shared
{
    public interface IServiceBus
    {
        void Send(object msg);
        void Publish(object msg);
    }
}