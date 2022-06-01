//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;

namespace Janacek
{
    /// <summary>
    /// Azure Queue channel configuraiton options.
    /// </summary>
    public class QueueChannelOptions
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        public string QueueName { get; set; }
    }

    /// <summary>
    /// Azure Queue channel receiver configuraiton options.
    /// </summary>
    public class QueueChannelReceiverOptions : QueueChannelOptions
    {
        /// <summary>
        /// Gets or sets the time delay between checking for messages.
        /// </summary>
        public TimeSpan SleepBetweenMessages { get; set; }
    }
}
