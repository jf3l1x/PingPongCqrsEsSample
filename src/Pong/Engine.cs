using System;
using Owin;
using PingPong.Shared;

namespace Pong
{
    public  class Engine : IModuleEngine
    {
        private readonly IModuleConfiguration _configuration;

        public Engine(IModuleConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IAppBuilder RegisterApi(IAppBuilder config)
        {
            return config;
        }

        public void StartListener()
        {
            Console.WriteLine(_configuration);
        }
    }
}