using Ping.Messages.Commands;

namespace Ping.Handlers.Commands
{
    public class CmdHandler:IHandle<StartPing>
    {
        public void Handle(StartPing cmd)
        {
            throw new System.NotImplementedException();
        }
    }
}