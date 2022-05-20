using System.Threading.Tasks;

namespace Janacek
{
    /// <summary>
    /// Interface for channel sender.
    /// </summary>
    public interface IChannelSender
    {
        /// <summary>
        /// Gets the channel name (part of channel identity).
        /// </summary>
        string ChannelName { get;  }

        /// <summary>
        /// Gets the channel address (part of channel identity).
        /// </summary>
        string ChannelAddress { get;  }

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