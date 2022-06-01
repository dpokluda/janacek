//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Janacek
{
    /// <summary>
    /// Simple Azure Queue channel receiver.
    /// </summary>
    /// <seealso cref="T:Janacek.IChannelReceiver"/>
    public class QueueChannelReceiver : IChannelReceiver
    {
        /// <summary>
        /// Host name.
        /// </summary>
        private const string HostName = "Queue";

        /// <summary>
        /// The check cancellation token in milliseconds.
        /// </summary>
        private const int DefaultSleepBetweenMessages = 5000;

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
        private QueueChannelReceiverOptions _options;

        /// <summary>
        /// Initializes a new instance of the Janacek.QueueChannelReceiver class.
        /// </summary>
        /// <param name="configureOptions">Options for controlling the configure.</param>
        public QueueChannelReceiver(Action<QueueChannelReceiverOptions> configureOptions)
        {
            // configure options
            _options = new QueueChannelReceiverOptions()
            {
                SleepBetweenMessages = TimeSpan.FromMilliseconds(DefaultSleepBetweenMessages)
            };
            configureOptions(_options);
            if (_options == null)
            {
                throw new ArgumentNullException("options");
            }
            else if (_options.SleepBetweenMessages == default(TimeSpan))
            {
                _options.SleepBetweenMessages = TimeSpan.FromMilliseconds(DefaultSleepBetweenMessages);
            }

            _storageAccount = CloudStorageAccount.Parse(_options.ConnectionString);

            _queueClient = _storageAccount.CreateCloudQueueClient();
            _queue = _queueClient.GetQueueReference(_options.QueueName);
            _queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            IsRunning = false;
        }

        /// <summary>
        /// Gets the channel name (part of channel identity).
        /// </summary>
        /// <seealso cref="P:Janacek.IChannelReceiver.HostName"/>
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
        /// Gets a value indicating whether this channel receiver is currently listening for incoming messages.
        /// </summary>
        /// <value>
        /// True if the channel is running, false if not.
        /// </value>
        /// <seealso cref="P:Janacek.IChannelReceiver.IsRunning"/>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Starts the channel receiver to listen for incoming messages.
        /// </summary>
        /// <exception cref="TransactionException">Thrown when a Transaction error condition occurs.</exception>
        /// <param name="onReceive">Functor to be executed whenever a message is received.</param>
        /// <param name="token">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        /// <seealso cref="M:Janacek.IChannelReceiver.StartAsync(Func{Message,IChannelReceiver,Task{Message}},CancellationToken)"/>
        public async Task StartAsync(Func<Message, IChannelReceiver, Task<Message>> onReceive, CancellationToken token = default(CancellationToken))
        {
            if (IsRunning)
            {
                throw new TransactionException("Channel receiver is already running.");
            }

            // start queue listener
            IsRunning = true;

            while (!token.IsCancellationRequested)
            {
                var queueMessage = await _queue.GetMessageAsync();
                if (queueMessage != null)
                {
                    Message requestMsg = null;
                    try
                    {
                        requestMsg = Message.Deserialize(queueMessage.AsString);
                    }
                    catch (Exception)
                    {
                        // can't deserialize received message
                    }

                    // process the request
                    await onReceive(requestMsg, this);
                    // delete the request from the queue
                    await _queue.DeleteMessageAsync(queueMessage);
                }
                else
                {
                    await Task.Delay(_options.SleepBetweenMessages, token);
                }
            }

            IsRunning = false;

        }

        /// <summary>
        /// Stops this channel receiver to listen for incoming messages.
        /// </summary>
        /// <seealso cref="M:Janacek.IChannelReceiver.Stop()"/>
        public void Stop()
        { }
    }
}
