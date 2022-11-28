using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Dominisoft.Nokates.Common.Infrastructure.Attributes;
using Dominisoft.Nokates.Common.Infrastructure.Configuration;
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
            return ServiceStatusHelper.GetServices(root).ToArray();


        }
        [HttpGet("EndpointGroups")]
        [EndpointGroup("System Admin")]
        public ActionResult<Dictionary<string, List<string>>> GetEndpointGroups()
        {
            return ServiceStatusHelper.GetGroups($"{Request.Scheme}://{Request.Host.Value}/");
        }

        [HttpGet("favicon.ico")]
        [NoAuth]
        public FileContentResult GetFavIcon()
        {
             ConfigurationValues.Values.TryGetValue("IconPath", out var path);
             return new FileContentResult(System.IO.File.ReadAllBytes(path ?? "./favicon.ico"), "image/x-icon");
        }

    }
}
