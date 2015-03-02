using System;

namespace PingPong.Shared
{
    public class VoidBus : IServiceBus
    {
        public void Publish(object msg)
        {
            Console.WriteLine(msg.GetType().FullName);
        }
    }
}