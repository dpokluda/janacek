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
        /// Name of this channel.
        /// </summary>
        private const string ThisChannelName = "Queue";

        /// <summary>
        /// The check cancellation token in milliseconds.
        /// </summary>
        private const int CheckCancellationTokenMilliseconds = 5000;

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
        /// Initializes a new instance of the Janacek.QueueChannelReceiver class.
        /// </summary>
        /// <param name="configureOptions">Options for controlling the configure.</param>
        public QueueChannelReceiver(Action<QueueChannelOptions> configureOptions)
        {
            // configure options
            _options = new QueueChannelOptions();
            configureOptions(_options);

            ChannelAddress = _options.QueueName;
            _storageAccount = CloudStorageAccount.Parse(_options.ConnectionString);

            _queueClient = _storageAccount.CreateCloudQueueClient();
            _queue = _queueClient.GetQueueReference(ChannelAddress);
            _queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            IsRunning = false;
        }

        /// <summary>
        /// Gets the channel name (part of channel identity).
        /// </summary>
        /// <seealso cref="P:Janacek.IChannelReceiver.ChannelName"/>
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
        /// <value>
        /// The address.
        /// </value>
        /// <seealso cref="P:Janacek.IChannelReceiver.ChannelAddress"/>
        public string ChannelAddress { get; private set; }

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
                    var message = Message.Deserialize(queueMessage.AsString);
                    var response = await onReceive(message, this);
                    await _queue.DeleteMessageAsync(queueMessage);
                }
                else
                {
                    await Task.Delay(CheckCancellationTokenMilliseconds, token);
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
