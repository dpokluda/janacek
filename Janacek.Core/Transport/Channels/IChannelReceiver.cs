//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Janacek
{
    /// <summary>
    /// Interface for channel receiver.
    /// </summary>
    public interface IChannelReceiver
    {
        /// <summary>
        /// Gets the channel host name (part of channel identity).
        /// </summary>
        string Host { get; }

        /// <summary>
        /// Serialize the channel into a Message.
        /// </summary>
        /// <returns>
        /// Message containing channel data.
        /// </returns>
        Message Serialize();

        /// <summary>
        /// Gets a value indicating whether this channel receiver is currently listening for incoming messages.
        /// </summary>
        /// <value>
        /// <c>True</c> if the channel is running; otherwise, <c>false</c>.
        /// </value>
        bool IsRunning { get; }

        /// <summary>
        /// Starts the channel receiver to listen for incoming messages.
        /// </summary>
        /// <param name="onReceive">Functor to be executed whenever a message is received.</param>
        /// <param name="token">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task StartAsync(Func<Message, IChannelReceiver, Task<Message>> onReceive, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Stops this channel receiver to listen for incoming messages.
        /// </summary>
        void Stop();
    }
}