//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System.Threading.Tasks;

namespace Janacek
{
    /// <summary>
    /// Interface for channel sender.
    /// </summary>
    public interface IChannelSender
    {
        /// <summary>
        /// Gets the channel host name (part of channel identity).
        /// </summary>
        string Host { get;  }

        /// <summary>
        /// Serialize the channel into a Message.
        /// </summary>
        /// <returns>
        /// Message containing channel data.
        /// </returns>
        Message Serialize();
        
        /// <summary>
        /// Sends an asynchronous message through the channel.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>
        /// An asynchronous result with the response message.
        /// </returns>
        Task<Message> SendAsync(Message message);
    }
}