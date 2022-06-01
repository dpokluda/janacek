//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

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
        /// Host name.
        /// </summary>
        private const string HostName = "Queue";

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
            if (_options == null)
            {
                throw new ArgumentNullException("options");
            }

            _storageAccount = CloudStorageAccount.Parse(_options.ConnectionString);

            _queueClient = _storageAccount.CreateCloudQueueClient();
            _queue = _queueClient.GetQueueReference(_options.QueueName);
            _queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the channel name (part of channel identity).
        /// </summary>
        /// <seealso cref="P:Janacek.IChannelSender.HostName"/>
        public string Host
        {
            get
            {
                return HostName;
            }
        }

        /// <summary>
        /// Gets the queue name (part of channel identity).
        /// </summary>
        public string QueueName
        {
            get
            {
                return _options.QueueName;
            }
        }

        /// <summary>
        /// Serialize the channel into a Message.
        /// </summary>
        /// <returns>
        /// Message containing channel data.
        /// </returns>
        /// <seealso cref="M:Janacek.IChannelReceiver.Serialize()"/>
        public Message Serialize()
        {
            return new Message
            {
                ["host"] = Host,
                ["queueName"] = QueueName,
                ["connectionString"] = _options.ConnectionString
            };
        }

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
