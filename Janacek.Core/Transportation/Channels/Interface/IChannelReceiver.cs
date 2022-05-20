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
        /// Gets the channel name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string ChannelName { get; }

        /// <summary>
        /// Gets the channel address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        string ChannelAddress { get; }

        /// <summary>
        /// Gets a value indicating whether this channel receiver is currently listening for incoming messages.
        /// </summary>
        /// <value>
        /// True if the channel is running, false if not.
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