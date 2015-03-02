using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace PingPong.WebHost
{
    class Program
    {
        static void Main(string[] args)
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
