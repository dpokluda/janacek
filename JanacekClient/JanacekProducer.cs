using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JanacekClient
{
    public class JanacekProducer
    {
        public async Task<Message> Act(Message msg)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // call prague service
            var serviceUrl = "http://localhost:65445/janacek/act";

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, serviceUrl);
            string serialized = JsonConvert.SerializeObject(msg);
            requestMessage.Content = new StringContent(serialized, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

            // process result
            if (responseMessage.Content != null)
            {
                string response = await responseMessage.Content.ReadAsStringAsync();
                // TODO: some checking needs to be done here (type checking)
                var outgoingMessage = JsonConvert.DeserializeObject<Message>(response);
                return outgoingMessage;
            }

            return new Message(new Dictionary<string, object>());
        }
    }
}
