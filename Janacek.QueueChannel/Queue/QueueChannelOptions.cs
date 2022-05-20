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
}
