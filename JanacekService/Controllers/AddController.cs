using System.Collections.Generic;
using JanacekClient;
using Microsoft.AspNetCore.Mvc;

namespace JanacekService.Controllers
{
    [Route("janacek/add")]
    [ApiController]
    public class AddController : ControllerBase
    {
        [HttpPost]
        public ActionResult Post([FromBody] Message msg)
        {
            var pattern = (string)msg["pattern"];
            var service = (string)msg["service"];
            JanacekService.Janacek.Add(pattern, message => new Message(new Dictionary<string, object>
            {
                { "service", service }
            }));

            return Ok();
        }
    }
}