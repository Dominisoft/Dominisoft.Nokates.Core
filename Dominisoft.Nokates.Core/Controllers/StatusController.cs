using System.Collections.Generic;
using Dominisoft.Nokates.Common.Infrastructure.Helpers;
using Dominisoft.Nokates.Common.Models;
using Dominisoft.Nokates.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Dominisoft.Nokates.Core.Controllers
{
    [Route("")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        [HttpGet("")]
        public ActionResult<ServiceStatus[]> Get()
        {
            var StatusResults = ServiceStatusHelper.GetServices(AppHelper.GetRootUri());
            return StatusResults.ToArray();
        }
        [HttpGet("EndpointGroups")]
        public ActionResult<Dictionary<string, List<string>>> GetEndpointGroups()
        {
            return ServiceStatusHelper.GetGroups($"{Request.Scheme}://{Request.Host.Value}/");
        }
    }
}
