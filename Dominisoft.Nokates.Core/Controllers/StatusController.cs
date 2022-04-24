using System.Collections.Generic;
using System.Linq;
using Dominisoft.Nokates.Common.Infrastructure.Attributes;
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
        [EndpointGroup("System Admin")]
        public ActionResult<ServiceStatus[]> GetAllServiceStatuses()
        {
            var root = AppHelper.GetRootUri();
            var paths = ServiceStatusHelper.GetApplicationStatusPagePaths(root);
            var results = new List<ServiceStatus>();
            foreach (var path in paths)
            {
                var status = ServiceStatusHelper.GetStatus(path);
                results.Add(status);
            }

            return results.ToArray();
        }
        [HttpGet("EndpointGroups")]
        [EndpointGroup("System Admin")]
        public ActionResult<Dictionary<string, List<string>>> GetEndpointGroups()
        {
            return ServiceStatusHelper.GetGroups($"{Request.Scheme}://{Request.Host.Value}/");
        }
    }
}
