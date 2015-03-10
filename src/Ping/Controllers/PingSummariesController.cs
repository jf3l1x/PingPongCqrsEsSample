using System.Linq;
using System.Threading.Tasks;
using System.Web.OData;
using System.Web.OData.Routing;
using Constant.Module.Interfaces.Persistence.ReadModel;
using Ping.Shared.Model.Read;

namespace Ping.Web.Controllers
{
    [ODataRoutePrefix("PingSummaries")]
    public class PingSummariesController : ODataController
    {
        private readonly IReadFromRepository<PingSummary> _repository;

        public PingSummariesController(IReadFromRepository<PingSummary> repository)
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