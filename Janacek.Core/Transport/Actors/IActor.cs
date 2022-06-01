//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Janacek
{
    /// <summary>
    /// Interface for transportation actor.
    /// </summary>
    public interface IActor
    {
        /// <summary>
        /// Register a client to send messages through. This method
        /// registers a default sender in case there is no explicit pattern sender.
        /// </summary>
        /// <param name="sender">Sender channel.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        IActor Client(IChannelSender sender);

        /// <summary>
        /// Register a client to send messages through. This method
        /// registers a sender for a given message pattern.
        /// </summary>
        /// <param name="sender">Sender channel.</param>
        /// <param name="pattern">Message pattern.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        IActor Client(IChannelSender sender, string pattern);

        /// <summary>
        /// Send message.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <returns>
        /// An asynchronous result that yields the response message (in case of a sync channel).
        /// </returns>
        Task<Message> ActAsync(Message message);

        /// <summary>
        /// Map receiver/listener pattern to a local function. This is just adding new pattern, function pair
        /// to the collection of pending maps. You still have to properly assign the pending map 
        /// to the right receiver/listener channel.
        /// </summary>
        /// <param name="pattern">Request message pattern.</param>
        /// <param name="syncAction">Local function to process the request message.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        IActor Add(string pattern, Func<Message, Task<Message>> syncAction);

        /// <summary>
        /// Assign the pending pattern, function pairs to the specified receiver/listener channel. 
        /// This also resets pending patterns.
        /// </summary>
        /// <param name="receiver">Message channel receiver.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        IActor Listen(IChannelReceiver receiver);

        /// <summary>
        /// Reinterpret transportation actor as a concrete transportation actor.
        /// This is a helper method for building readable method chains.
        /// </summary>
        /// <typeparam name="T">Concrete transportation actor type.</typeparam>
        /// <returns>
        /// Concrete implementation of transportation actor.
        /// </returns>
        T TreatAs<T>() where T : IActor;

        /// <summary>
        /// Build the pipeline by properly connecting all channels together.
        /// </summary>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        IActor Build();

        /// <summary>
        /// Start the pipeline by starting all channel receivers for example.
        /// </summary>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        IActor Run();
    }
}
