using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Ping.Handlers;
using Ping.Messages.Commands;
using Ping.Model.Read;
using PingPong.Shared;

namespace Ping.Controllers
{
    
    public class PingController : ApiController
    {
        private readonly ICreateCmdHandle _cmdHandleFactory;
        private readonly IReadModelRepository<PingSummary> _repository;

        public PingController(ICreateCmdHandle cmdHandleFactory,IReadModelRepository<PingSummary> repository)
        {
            _cmdHandleFactory = cmdHandleFactory;
            _repository = repository;
        }

        [Route("Ping")]
        public Task<string> Get()
        {
            return Task.FromResult("ping");
        }
        
        [HttpGet]
        [Route("List")]
        public Task<IEnumerable<PingSummary>> List()
        {
            return Task.FromResult(_repository.Query().AsEnumerable());
        }
        [Route("Start")]
        [HttpPost]
        public Task<Guid> Start(int countLimit, int timeLimit)
        {
            var cmd = new StartPing()
            {
                CountLimit = countLimit,
                TimeLimit = TimeSpan.FromSeconds(timeLimit),
                AggregateId = Guid.NewGuid()
            };
            _cmdHandleFactory.Create<StartPing>().Handle(cmd);
            return Task.FromResult(cmd.AggregateId);

        }
        [Route("Stop")]
        [HttpPost]
        public Task Stop(Guid id)
        {
            var cmd = new StopPing()
            {

                AggregateId = id
            };
            _cmdHandleFactory.Create<StopPing>().Handle(cmd);
            return Task.FromResult(0);

        }
    }
}