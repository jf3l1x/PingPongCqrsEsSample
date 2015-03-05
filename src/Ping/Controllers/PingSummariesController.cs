using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using Ping.Model.Read;
using PingPong.Shared;

namespace Ping.Controllers
{
    [ODataRoutePrefix("PingSummaries")]
    public class PingSummariesController : ODataController
    {
        private readonly IReadModelRepository<PingSummary> _repository;

        public PingSummariesController(IReadModelRepository<PingSummary> repository)
        {
            _repository = repository;
        }

        [EnableQuery]
        [ODataRoute]
        public Task<IQueryable<PingSummary>> Get()
        {
            return Task.FromResult(_repository.Query());
        }

      
    }
}