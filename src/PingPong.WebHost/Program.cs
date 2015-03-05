using System;
using Microsoft.Owin.Hosting;

namespace PingPong.WebHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Need run as admin.
            var options = new StartOptions("http://*:9999")
            {
                ServerFactory = "Microsoft.Owin.Host.HttpListener"
            };
            
            using (WebApp.Start<Startup>(options))
            {
                Console.WriteLine("\n\nAll Applications...");
                Console.WriteLine("\n\nServer listening . Press enter to stop");
                Console.ReadLine();
            }
        }
    }
}