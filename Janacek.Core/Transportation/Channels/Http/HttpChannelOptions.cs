using System;

namespace Janacek
{
    /// <summary>
    /// HTTP channel options.
    /// </summary>
    public abstract class HttpChannelOptions
    {
        /// <summary>
        /// Gets or sets the HTTP channel service address.
        /// </summary>
        public string ServiceAddress { get; set; }
    }

    /// <summary>
    /// HTTP channel sender options.
    /// </summary>
    /// <seealso cref="T:Janacek.HttpChannelOptions"/>
    public class HttpChannelSenderOptions : HttpChannelOptions
    {
        /// <summary>
        /// Initializes a new instance of the HttpChannelSenderOptions class.
        /// </summary>
        public HttpChannelSenderOptions()
        {
            Timeout = System.Threading.Timeout.InfiniteTimeSpan;
        }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        public TimeSpan Timeout { get; set; }
    }

    /// <summary>
    /// HTTP channel receiver options.
    /// </summary>
    /// <seealso cref="T:Janacek.HttpChannelOptions"/>
    public class HttpChannelReceiverOptions : HttpChannelOptions
    {
        /// <summary>
        /// Gets or sets the interval to check for cancellation token to be signaled.
        /// </summary>
        public TimeSpan CheckCancellationTokenTimeSpan { get; set; }
    }
}
