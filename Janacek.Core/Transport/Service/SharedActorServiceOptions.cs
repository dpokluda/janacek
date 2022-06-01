//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

namespace Janacek.Service
{
    /// <summary>
    /// Shared actor service options.
    /// </summary>
    public class SharedActorServiceOptions
    {
        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        public IChannelSender Sender { get; set; }

        /// <summary>
        /// Gets or sets the receiver.
        /// </summary>
        public IChannelReceiver Receiver { get; set; }

        /// <summary>
        /// Gets or sets a boolean flag indicating whether exception details will be included in the response message.
        /// </summary>
        /// <value>
        /// <c>True</c> if exception details are included; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeExceptionDetails { get; set; }
        
        /// <summary>
        /// Gets or sets a boolean flag indicating whether a debug output
        /// should be generated during execution for better diagnostics.
        /// </summary>
        /// <value>
        /// <c>True</c> if debug output is generated; otherwise, <c>false</c>.
        /// </value>
        public bool DebugOutput { get; set; }
    }
}
