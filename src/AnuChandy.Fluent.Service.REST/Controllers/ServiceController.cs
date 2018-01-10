// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using AnuChandy.Fluent.Service.Model;
using AnuChandy.Fluent.Service.Model.Core.Channel;
using AnuChandy.Fluent.Service.REST.Model;
using System;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.REST.Controllers
{
    [Route("api")]
    public class ServiceController : Controller
    {
        [HttpPost("BeginCreate")]
        public async Task<IActionResult> Post([FromBody]FluentRequestModel fluentRequestPayload)
        {
            if (fluentRequestPayload == null)
            {
                return BadRequest(new Exception("Failed to parse the json payload, make sure it is valid"));
            }

            var channel = await RequestChannel.CreateAsync();
            var requestId = await channel.WriteAsync(fluentRequestPayload);

            var pollingUrl = Link($"GetCreateStatus/{requestId}");
            HttpContext.Response.Headers["Location"] = pollingUrl;

            return Ok(new BeginCreateResponse(pollingUrl));
        }

        [HttpGet("GetCreateStatus/{requestId}")]
        public async Task<IActionResult> Get(String requestId)
        {
            var table = await  ResourceCreateStatusesTable.CreateAsync(requestId);
            // Creates response model instance and return.
            //
            return Ok(PollingResponse.From(await table.GetStatusesAsync()));
        }

        private string Link(string route)
        {
           return $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/{route}";
        } 
    }
}
