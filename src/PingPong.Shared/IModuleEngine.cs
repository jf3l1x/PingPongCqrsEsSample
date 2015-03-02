using Owin;

namespace PingPong.Shared
{
    public interface IModuleEngine
    {
        IAppBuilder RegisterApi(IAppBuilder config);
        void StartListener();

    }
}