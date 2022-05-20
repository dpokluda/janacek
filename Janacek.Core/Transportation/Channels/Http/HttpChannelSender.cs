using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Janacek
{
    /// <summary>
    /// Simple (naive) HTTP channel sender.
    /// </summary>
    /// <seealso cref="T:Janacek.IChannelSender"/>
    public class HttpChannelSender : IChannelSender
    {
        /// <summary>
        /// Name of this channel.
        /// </summary>
        private const string ThisChannelName = "Http";

        /// <summary>
        /// The client.
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>
        /// Configuration options.
        /// </summary>
        private readonly HttpChannelSenderOptions _options;

        /// <summary>
        /// Initializes a new instance of the HttpChannelSender class.
        /// </summary>
        /// <param name="configureOptions">Configuration options.</param>
        public HttpChannelSender(Action<HttpChannelSenderOptions> configureOptions)
        {
            // configure options
            _options = new HttpChannelSenderOptions();
            configureOptions(_options);

            ChannelAddress = _options.ServiceAddress;

            _client = new HttpClient();
            if (_options != null && _options.Timeout != default(TimeSpan))
            {
                _client.Timeout = _options.Timeout;
            }
            _client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Gets the channel name (part of channel identity).
        /// </summary>
        /// <seealso cref="P:Janacek.IChannelSender.ChannelName"/>
        public string ChannelName
        {
            get
            {
                return ThisChannelName;
            }
        }

        /// <summary>
        /// Gets the channel address (part of channel identity).
        /// </summary>
        /// <seealso cref="P:Janacek.IChannelSender.ChannelAddress"/>
        public string ChannelAddress { get; private set; }

        /// <summary>
        /// Sends an asynchronous message through the channel.
        /// </summary>
        /// <exception cref="TransportationException">Thrown when a transport independence exception occurs.</exception>
        /// <param name="message">The message to send.</param>
        /// <returns>
        /// An asynchronous result with the response message.
        /// </returns>
        /// <seealso cref="M:Janacek.IChannelSender.SendAsync(Message)"/>
        public virtual async Task<Message> SendAsync(Message message)
        {
            var requestString = message.Serialize();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, ChannelAddress);
            requestMessage.Content = new StringContent(requestString, Encoding.UTF8);
            HttpResponseMessage responseMessage = await _client.SendAsync(requestMessage);

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                throw new TransportationException("Transportation channel unexpected response");
            }

            if (responseMessage.Content != null)
            {
                string responseString = responseMessage.Content.ReadAsStringAsync().Result;
                var responseMsg = Message.Deserialize(responseString);
                return responseMsg;
            }
            else
            {
                return Message.Empty;
            }
        }
    }
}
