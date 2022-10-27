//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Janacek
{
    /// <summary>
    /// Simple (naive) UDP channel receiver.
    /// </summary>
    /// <seealso cref="T:System.IDisposable"/>
    /// <seealso cref="T:Janacek.IChannelReceiver"/>
    public class UdpChannelReceiver : IChannelReceiver, IDisposable
    {
        /// <summary>
        /// Host name.
        /// </summary>
        private const string HostName = "Udp";

        /// <summary>
        /// Default interval to check for cancellation token to be signaled.
        /// </summary>
        private const int CheckCancelTokenInMilliseconds = 5000;

        /// <summary>
        /// UDP client
        /// </summary>
        private UdpClient _client;

        /// <summary>
        /// Configuration options.
        /// </summary>
        private readonly UdpChannelReceiverOptions _options;

        /// <summary>
        /// Initializes a new instance of the HttpChannelReceiver class.
        /// </summary>
        /// <param name="configureOptions">Configuration options.</param>
        public UdpChannelReceiver(Action<UdpChannelReceiverOptions> configureOptions)
        {
            // set configuration options
            _options = new UdpChannelReceiverOptions();
            configureOptions(_options);
            if (_options == null)
            {
                throw new ArgumentNullException("options");
            }

            _client = null;
            IsRunning = false;
        }

        /// <summary>
        /// Gets the channel name (part of channel identity).
        /// </summary>
        /// <seealso cref="P:Janacek.IChannelReceiver.Host"/>
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
        /// <seealso cref="P:Janacek.IChannelReceiver.Port"/>
        public string Port
        {
            get
            {
                return _options.Address;
            }
        }

        /// <summary>
        /// Starts the channel receiver to listen for incoming messages.
        /// </summary>
        /// <exception cref="TransportationException">Thrown when a transportation independence exception occurs.</exception>
        /// <param name="onReceive">Functor to be executed whenever a message is received.</param>
        /// <param name="token">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        /// <seealso cref="M:Janacek.IChannelReceiver.StartAsync(Func{Message,IChannelReceiver,Task{Message}},CancellationToken)"/>
        public async Task StartAsync(Func<Message, IChannelReceiver, Task<Message>> onReceive, CancellationToken token = default(CancellationToken))
        {
            if (IsRunning)
            {
                throw new TransportationException("Channel receiver is already running.");
            }

            if (_client == null)
            {
                var uri = new Uri(_options.Address);
                _client = new UdpClient(uri.Port);
            }

            // configure default remote host
            IsRunning = true;

            // wait for message/request
            var receiveTask = _client.ReceiveAsync();
            TimeSpan checkDelay;
            if (_options != null && _options.CheckCancellationTokenTimeSpan != default(TimeSpan))
            {
                checkDelay = _options.CheckCancellationTokenTimeSpan;
            }
            else
            {
                checkDelay = TimeSpan.FromMilliseconds(CheckCancelTokenInMilliseconds);
            }
            while (token != null && !token.IsCancellationRequested)
            {
                // start a delay task so that we have a chance to regularly check the cancellation token
                var delayTask = Task.Delay(checkDelay, token);
                var resultTask = await Task.WhenAny(receiveTask, delayTask);
                if (resultTask == receiveTask)
                {
                    // on request - process it
                    UdpReceiveResult receiveResult = await receiveTask;
                    await ProcessRequest(receiveResult, onReceive);

                    // wait for another request
                    receiveTask = _client.ReceiveAsync();
                }
            }

            IsRunning = false;
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
        /// Gets a value indicating whether this channel receiver is currently listening for incoming messages.
        /// </summary>
        /// <value>
        /// True if the channel is running, false if not.
        /// </value>
        /// <seealso cref="P:Janacek.IChannelReceiver.IsRunning"/>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="result">Received result.</param>
        /// <param name="onReceive">The on receive.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        private async Task ProcessRequest(UdpReceiveResult result, Func<Message, IChannelReceiver, Task<Message>> onReceive)
        {
            if (result != null && onReceive != null)
            {
                var buffer = result.Buffer;
                if (buffer != null && buffer.Length > 0)
                {
                    Message requestMsg = null;
                    try
                    {
                        var requestString = Encoding.UTF8.GetString(buffer);
                        requestMsg = Message.Deserialize(requestString);
                    }
                    catch (Exception)
                    {
                        // if we can't deserialize the datagram then 
                        // it probably something we don't care about.
                    }
    
                    await onReceive(requestMsg, this);
                }
            }
        }

        /// <summary>
        /// Stops this channel receiver to listen for incoming messages.
        /// </summary>
        /// <seealso cref="M:Janacek.IChannelReceiver.Stop()"/>
        public void Stop()
        {
            IsRunning = false;
        }

        /// <summary>
        /// Closes this HttpChannelReceiver.
        /// </summary>
        public void Close()
        {
            Stop();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <seealso cref="M:System.IDisposable.Dispose()"/>
        public void Dispose()
        {
            if (_client != null)
            {
                Stop();
                _client.Close();
            }
        }
    }
}
