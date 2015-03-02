using System;
using Microsoft.Owin.Hosting;

namespace PingPong.WebHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string url = "http://localhost:9999";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("\n\nAll Applications...");
                Console.WriteLine("\n\nServer listening at {0}. Press enter to stop", url);
                Console.ReadLine();
            }
        }
    }
}