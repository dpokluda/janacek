using System.Collections.Generic;
using JanacekClient;
using Microsoft.AspNetCore.Mvc;

namespace ServiceA.Controllers
{
    [Route("test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // GET
        [HttpGet]
        public ActionResult<Message> Get()
        {
            return new ActionResult<Message>(new Message(new Dictionary<string, object>
            {
                { "Service", "ServiceA" },
                { "Status", ":-)" }
            }));
        }
    }
}
