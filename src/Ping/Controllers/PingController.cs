using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Constant.Module.Interfaces.Bus;
using Constant.Module.Interfaces.Persistence.ReadModel;
using Ping.Shared.Messages.Commands;
using Ping.Shared.Model.Read;

namespace Ping.Web.Controllers
{
    public class PingController : ApiController
    {
        private readonly ISendMessages _msgSender;
        private readonly IReadFromRepository<PingSummary> _repository;

        public PingController(ISendMessages msgSender, IReadFromRepository<PingSummary> repository)
        {
            _msgSender = msgSender;
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
            var cmd = new StartPing
            {
                CountLimit = countLimit,
                TimeLimit = TimeSpan.FromSeconds(timeLimit),
                AggregateId = Guid.NewGuid()
            };
            _msgSender.Send(cmd);
            return Task.FromResult(cmd.AggregateId);
        }

        [Route("Stop")]
        [HttpPost]
        public Task Stop(Guid id)
        {
            var cmd = new StopPing
            {
                AggregateId = id
            };
            _msgSender.Send(cmd);
            return Task.FromResult(0);
        }
    }
}