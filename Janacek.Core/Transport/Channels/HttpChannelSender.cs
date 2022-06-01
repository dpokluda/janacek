//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Janacek
{
    /// <summary>
    /// Simple (naive) HTTP channel sender.
    /// </summary>
    /// <seealso cref="T:Janacek.IChannelSender"/>
    public class HttpChannelSender : IChannelSender, IDisposable
    {
        /// <summary>
        /// Host name.
        /// </summary>
        private const string HostName = "Http";

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
            if (_options == null)
            {
                throw new ArgumentNullException("options");
            }

            // setup channel client
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
        /// <seealso cref="P:Janacek.IChannelSender.Host"/>
        public string Host
        {
            get
            {
                return HostName;
            }
        }

        /// <summary>
        /// Gets the channel address (part of channel identity).
        /// </summary>
        /// <seealso cref="P:Janacek.IChannelSender.Port"/>
        public string Port
        {
            get
            {
                return _options.Address;
            }
        }

        /// <summary>
        /// Serialize the channel into a Message.
        /// </summary>
        /// <returns>
        /// Message containing channel data.
        /// </returns>
        /// <seealso cref="M:Janacek.IChannelReceiver.Serialize()"/>
        public Message Serialize()
        {
            return new Message
            {
                ["host"] = Host,
                ["port"] = Port
            };
        }

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
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, Port);
            var requestString = message.Serialize();
            requestMessage.Content = new StringContent(requestString, Encoding.UTF8);
            HttpResponseMessage responseMessage = await _client.SendAsync(requestMessage);
            string content = await GetResponseContent(responseMessage);

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                string exception = "Transportation channel unexpected response.";
                if (!string.IsNullOrEmpty(content))
                {
                    exception += $"Details: {content}";
                }
                throw new TransportationException(exception);
            }

            if (!string.IsNullOrEmpty(content))
            {
                return Message.Deserialize(content);
            }
            else
            {
                return Message.Empty;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <seealso cref="M:System.IDisposable.Dispose()"/>
        public void Dispose()
        {
            _client?.Dispose();
        }

        /// <summary>
        /// Extract response content.
        /// </summary>
        /// <param name="response">The response message.</param>
        /// <returns>
        /// An asynchronous result that yields the response content.
        /// </returns>
        private async Task<string> GetResponseContent(HttpResponseMessage response)
        {
            if (response.Content != null)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
}
