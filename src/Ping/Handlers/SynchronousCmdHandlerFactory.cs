using Ping.Handlers.Commands;

namespace Ping.Handlers
{
    public class SynchronousCmdHandlerFactory : ICreateCmdHandle
    {
        private readonly CmdHandler _cmdHandler;


        public SynchronousCmdHandlerFactory(CmdHandler cmdHandler)
        {
            _cmdHandler = cmdHandler;
        }

        public IHandle<T> Create<T>()
        {
            return (IHandle<T>)_cmdHandler;
        }
    }
}