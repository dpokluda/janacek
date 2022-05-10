using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JanacekClient
{
    public class JanacekConsumer
    {
        private readonly Dictionary<string, Janacek> _registeredServices;
        private readonly List<KeyValuePair<string, Func<Message, Message>>> _pending;

        public JanacekConsumer(Dictionary<string, Janacek> registeredServices)
        {
            _registeredServices = registeredServices;
            _pending = new List<KeyValuePair<string, Func<Message, Message>>>();
        }

        public JanacekConsumer Add(string pattern, Func<Message, Message> action)
        {
            _pending.Add(new KeyValuePair<string, Func<Message, Message>>(pattern, action));
            return this;
        }

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

        public JanacekConsumer Listen(string listenUrl)
        {
            var registeredService = new Janacek();
            foreach ((string pattern, Func<Message, Message> action) in _pending)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // call janacek service
                var serviceUrl = "http://localhost:65445/janacek/add";

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, serviceUrl);
                string serialized = JsonConvert.SerializeObject(new Message(new Dictionary<string, object>
                {
                    { "pattern", pattern },
                    { "service", listenUrl }
                }));

                requestMessage.Content = new StringContent(serialized, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.SendAsync(requestMessage).Result;

                // process result
                if (responseMessage.Content != null)
                {
                    string response = responseMessage.Content.ReadAsStringAsync().Result;
                }

                registeredService.Add(pattern, action);
            }

            _registeredServices.Add(listenUrl.ToLowerInvariant(), registeredService);
            return this;
        }
    }
}
