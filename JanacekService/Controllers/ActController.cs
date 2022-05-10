using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JanacekClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JanacekService.Controllers
{
    [Route("janacek/act")]
    [ApiController]
    public class ActController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Message>> Post([FromBody] Message msg)
        {
            var result = JanacekService.Janacek.Act(msg);
            if (result != null && result.ContainsKey("service"))
            {
                var service = (string)result["service"];

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, service);
                string serialized = JsonConvert.SerializeObject(msg);
                requestMessage.Content = new StringContent(serialized, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

                // process result
                if (responseMessage.Content != null)
                {
                    string response = await responseMessage.Content.ReadAsStringAsync();
                    // TODO: some checking needs to be done here (type checking)
                    var outgoingMessage = JsonConvert.DeserializeObject<Message>(response);
                    return Ok(outgoingMessage);
                }
                else
                {
                    return Ok();
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}