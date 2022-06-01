//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

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
        public string Address { get; set; }
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

        /// <summary>
        /// Gets or sets a boolean flag indicating whether exception details will be included in the response message.
        /// </summary>
        /// <value>
        /// <c>True</c> if exception details are included; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeExceptionDetails { get; set; }
    }
}
