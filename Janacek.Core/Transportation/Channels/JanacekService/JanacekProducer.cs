using System.Threading.Tasks;

namespace Janacek
{
    /// <summary>
    /// Client component used for routing messages through Janacek service.
    /// </summary>
    /// <seealso cref="T:Janacek.MessageProducer"/>
    public class JanacekProducer : MessageProducer
    {
        /// <summary>
        /// Initializes a new instance of the JanacekProducer class.
        /// </summary>
        public JanacekProducer()
        {
            // default sender sends all messages to Janacek
            Client(new JanacekServerClient());
        }

        /// <summary>
        /// Send message through pattern matching library to the corresponding service(s).
        /// </summary>
        /// <exception cref="TransportationException">Thrown when a transport independence exception occurs.</exception>
        /// <param name="message">The message to be sent.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public override async Task<Message> ActAsync(Message message)
        {
            var sender = PatternSenders.Match(message);
            if (sender != null)
            {
                return await base.ActAsync(message);
            }

            if (DefaultSender == null)
            {
                throw new TransportationException("No sender available for the message.");
            }

            return await DefaultSender.SendAsync(new Message
            {
                ["role"] = "janacek",
                ["cmd"] = "act",
                ["data"] = message
            });
        }
    }
}
