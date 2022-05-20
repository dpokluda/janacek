using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Janacek
{
    /// <summary>
    /// Simple (naive) HTTP channel receiver.
    /// </summary>
    /// <seealso cref="T:System.IDisposable"/>
    /// <seealso cref="T:Janacek.IChannelReceiver"/>
    public class HttpChannelReceiver : IDisposable, IChannelReceiver
    {
        /// <summary>
        /// Name of this channel.
        /// </summary>
        private const string ThisChannelName = "Http";

        /// <summary>
        /// Default interval to check for cancellation token to be signaled.
        /// </summary>
        private const int CheckCancelTokenInMilliseconds = 15000;

        /// <summary>
        /// HTTP listener.
        /// </summary>
        private HttpListener _listener;

        /// <summary>
        /// Configuration options.
        /// </summary>
        private readonly HttpChannelReceiverOptions _options;

        /// <summary>
        /// Initializes a new instance of the HttpChannelReceiver class.
        /// </summary>
        /// <param name="configureOptions">Configuration options.</param>
        public HttpChannelReceiver(Action<HttpChannelReceiverOptions> configureOptions)
        {
            // set configuration options
            _options = new HttpChannelReceiverOptions();
            configureOptions(_options);

            ChannelAddress = _options.ServiceAddress;

            _listener = null;
            IsRunning = false;
        }

        /// <summary>
        /// Gets the channel name (part of channel identity).
        /// </summary>
        /// <seealso cref="P:Janacek.IChannelReceiver.ChannelName"/>
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
        /// <seealso cref="P:Janacek.IChannelReceiver.ChannelAddress"/>
        public string ChannelAddress
        {
            get; private set;
        }

        /// <summary>
        /// Starts the channel receiver to listen for incoming messages.
        /// </summary>
        /// <exception cref="TransportationException">Thrown when a transportation idependence exception occurs.</exception>
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

            if (_listener == null)
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add(ChannelAddress);
            }

            // start HTTP channel receiver (server)
            IsRunning = true;
            _listener.Start();

            // wait for message/request
            var getContextTask = _listener.GetContextAsync();
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
                var delayTask = Task.Delay(checkDelay, token);
                var resultTask = await Task.WhenAny(getContextTask, delayTask);
                if (resultTask == getContextTask)
                {
                    // on request - process it
                    HttpListenerContext request = await getContextTask;
                    await ProcessRequest(request, onReceive);

                    getContextTask = _listener.GetContextAsync();
                }
            }

            _listener.Stop();
            IsRunning = false;
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
        /// <param name="context">The context.</param>
        /// <param name="onReceive">The on receive.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public async Task ProcessRequest(HttpListenerContext context, Func<Message, IChannelReceiver, Task<Message>> onReceive)
        {
            try
            {
                // extract request message
                HttpListenerRequest request = context.Request;
#pragma warning disable 4014
                if (request.HasEntityBody)
                {
                    using (var stream = request.InputStream)
                    using (var reader = new StreamReader(stream))
                    {
                        var requestString = await reader.ReadToEndAsync();
                        Message requestMsg = null;
                        try
                        {
                            requestMsg = Message.Deserialize(requestString);
                        }
                        catch (Exception exception)
                        {
                            // can't deserialize received message
                            await BadRequest(context);
                        }

                        var responseMsg = await onReceive(requestMsg, this);

                        var response = context.Response;
                        string responseString = responseMsg.Serialize();
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = buffer.Length;
                        System.IO.Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }
                }
                else
                {
                    await BadRequest(context);
                }
#pragma warning restore 4014
            }
            catch (Exception exception)
            {
                await InternalServerError(context);
            }
        }

        /// <summary>
        /// Respond with Bad request HTTP status.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected Task BadRequest(HttpListenerContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.Close();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Respond with Internal server error HTTP status.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected Task InternalServerError(HttpListenerContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Close();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops this channel receiver to listen for incoming messages.
        /// </summary>
        /// <seealso cref="M:Janacek.IChannelReceiver.Stop()"/>
        public void Stop()
        {
            _listener.Stop();
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
            if (_listener != null)
            {
                Stop();
                _listener.Close();
            }
        }
    }
}
