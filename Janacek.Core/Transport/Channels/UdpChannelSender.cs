//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Janacek
{
    /// <summary>
    /// Simple (naive) UDP channel sender.
    /// </summary>
    /// <seealso cref="T:Janacek.IChannelSender"/>
    public class UdpChannelSender : IChannelSender, IDisposable
    {
        /// <summary>
        /// Host name.
        /// </summary>
        private const string HostName = "Udp";

        /// <summary>
        /// The client.
        /// </summary>
        private readonly UdpClient _client;

        /// <summary>
        /// Configuration options.
        /// </summary>
        private readonly UdpChannelSenderOptions _options;

        /// <summary>
        /// Initializes a new instance of the HttpChannelSender class.
        /// </summary>
        /// <param name="configureOptions">Configuration options.</param>
        public UdpChannelSender(Action<UdpChannelSenderOptions> configureOptions)
        {
            // configure options
            _options = new UdpChannelSenderOptions();
            configureOptions(_options);
            if (_options == null)
            {
                throw new ArgumentNullException("options");
            }

            // create UDP client
            _client = new UdpClient();
            
            // configure default remote host
            var uri = new Uri(_options.Address);
            _client.Connect(uri.Host, uri.Port);
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

            var requestString = message.Serialize();
            var datagram = Encoding.UTF8.GetBytes(requestString);
            var bytesSent = await _client.SendAsync(datagram, datagram.Length);

            return Message.Empty;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <seealso cref="M:System.IDisposable.Dispose()"/>
        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
