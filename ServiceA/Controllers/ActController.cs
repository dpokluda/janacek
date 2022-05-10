using JanacekClient;
using Microsoft.AspNetCore.Mvc;

namespace ServiceA.Controllers
{
    [Route("janacek/act")]
    [ApiController]
    public class ActController : Controller
    {
        [HttpPost]
        public ActionResult<Message> Post([FromBody] Message msg)
        {
            string url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}".ToLowerInvariant();
            if (JanacekService.registeredServices.ContainsKey(url))
            {
                Janacek janacek = JanacekService.registeredServices[url];
                return Ok(janacek.Act(msg));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
