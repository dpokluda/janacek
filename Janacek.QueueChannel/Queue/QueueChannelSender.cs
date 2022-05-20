using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Janacek
{
    /// <summary>
    /// Simple Azure Queue channel sender.
    /// </summary>
    /// <seealso cref="T:Janacek.IChannelSender"/>
    public class QueueChannelSender : IChannelSender
    {
        /// <summary>
        /// Name of this channel.
        /// </summary>
        private const string ThisChannelName = "Queue";

        /// <summary>
        /// The storage account.
        /// </summary>
        private readonly CloudStorageAccount _storageAccount;

        /// <summary>
        /// The queue client.
        /// </summary>
        private CloudQueueClient _queueClient;

        /// <summary>
        /// The queue.
        /// </summary>
        private CloudQueue _queue;

        /// <summary>
        /// Options for controlling the operation.
        /// </summary>
        private QueueChannelOptions _options;

        /// <summary>
        /// Initializes a new instance of the QueueChannelSender class.
        /// </summary>
        /// <param name="configureOptions">The address.</param>
        public QueueChannelSender(Action<QueueChannelOptions> configureOptions)
        {
            // configure options
            _options = new QueueChannelOptions();
            configureOptions(_options);

            ChannelAddress = _options.QueueName;
            _storageAccount = CloudStorageAccount.Parse(_options.ConnectionString);

            _queueClient = _storageAccount.CreateCloudQueueClient();
            _queue = _queueClient.GetQueueReference(ChannelAddress);
            _queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the channel name (part of channel identity).
        /// </summary>
        /// <seealso cref="P:Janacek.IChannelSender.ChannelName"/>
        public string ChannelName
        {
            get
            {
                return ThisChannelName;
            }
        }

        /// <summary>
        /// Gets the channel address (part of channel identity).
        /// </summary>
        /// <seealso cref="P:Janacek.IChannelSender.ChannelAddress"/>
        public string ChannelAddress { get; }

        /// <summary>
        /// Sends an asynchronous message through the channel.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>
        /// An asynchronous result with the response message.
        /// </returns>
        /// <seealso cref="M:Janacek.IChannelSender.SendAsync(Message)"/>
        public async Task<Message> SendAsync(Message message)
        {
            CloudQueueMessage queueMessage = new CloudQueueMessage(message.Serialize());
            await _queue.AddMessageAsync(queueMessage);

            return Message.Empty;
        }
    }
}
