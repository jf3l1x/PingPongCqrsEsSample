using System;
using System.Linq;
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
        public IHttpActionResult GetFeed()
        {
            return Ok(_repository.Query());
        }

        [ODataRoute("({id})")]
        [EnableQuery]
        public IHttpActionResult GetEntity(Guid id)
        {
            return Ok(SingleResult.Create(_repository.Query().Where(t => t.Id == id)));
        }
    }
}