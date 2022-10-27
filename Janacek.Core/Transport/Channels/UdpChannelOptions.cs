//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;

namespace Janacek
{
    /// <summary>
    /// UDP channel options.
    /// </summary>
    public abstract class UdpChannelOptions
    {
        /// <summary>
        /// Gets or sets the HTTP channel service address.
        /// </summary>
        public string Address { get; set; }
    }

    /// <summary>
    /// UDP channel sender options.
    /// </summary>
    /// <seealso cref="T:Janacek.UdpChannelOptions"/>
    public class UdpChannelSenderOptions : UdpChannelOptions
    {
        /// <summary>
        /// Initializes a new instance of the HttpChannelSenderOptions class.
        /// </summary>
        public UdpChannelSenderOptions()
        {
        }
    }

    /// <summary>
    /// UDP channel receiver options.
    /// </summary>
    /// <seealso cref="T:Janacek.UdpChannelOptions"/>
    public class UdpChannelReceiverOptions : UdpChannelOptions
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
