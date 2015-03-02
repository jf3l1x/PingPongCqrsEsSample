using System;
using System.Threading.Tasks;
using System.Web.Http;
using Ping.Handlers;
using Ping.Messages.Commands;

namespace Ping.Controllers
{
    
    public class PingController : ApiController
    {
        private readonly ICreateHandle _handleFactory;

        public PingController(ICreateHandle handleFactory)
        {
            _handleFactory = handleFactory;
        }
        [Route("Ping")]
        public Task<string> Get()
        {
            return Task.FromResult("ping");
        }
        [Route("Start")]
        [HttpPost]
        public Task<Guid> Post(int countLimit, int timeLimit)
        {
            var cmd = new StartPing()
            {
                CountLimit = countLimit,
                TimeLimit = TimeSpan.FromSeconds(timeLimit),
                AggregateId = Guid.NewGuid()
            };
            _handleFactory.Create<StartPing>().Handle(cmd);
            return Task.FromResult(cmd.AggregateId);

        }
    }
}