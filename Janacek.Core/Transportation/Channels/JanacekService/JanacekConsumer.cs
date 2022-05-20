using System;
using System.Threading.Tasks;

namespace Janacek
{
    /// <summary>
    /// Helper component used when we want to be recipients for messages routed from Janacek service.
    /// </summary>
    /// <seealso cref="T:Janacek.MessageConsumer"/>
    public class JanacekConsumer : MessageConsumer
    {
        /// <summary>
        /// Initializes a new instance of the JanacekConsumer class.
        /// </summary>
        public JanacekConsumer()
        {
            Client(new JanacekServerClient());
        }

        /// <summary>
        /// Configure channel listener for all outstanding message patterns.
        /// </summary>
        /// <param name="receiver">The channel receiver.</param>
        /// <returns>
        /// An IMessageConsumer object.
        /// </returns>
        public override IMessageConsumer Listen(IChannelReceiver receiver)
        {
            foreach (PatternValuePair<Func<Message, Task<Message>>> pattern in _pendingPatterns.Patterns)
            {
                var addMessage = new Message
                {
                    ["role"] = "janacek",
                    ["cmd"] = "add",
                    ["pattern"] = pattern.Pattern.ToString(),
                    ["channel-name"] = receiver.ChannelName,
                    ["channel-address"] = receiver.ChannelAddress
                };
                var response = DefaultSender.SendAsync(addMessage).GetAwaiter().GetResult();
            }

            return base.Listen(receiver);
        }

        /// <summary>
        /// Route message using client channel for pattern matching.
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

            var actMessasge = new Message
            {
                ["role"] = "janacek",
                ["cmd"] = "act",
                ["data"] = message
            };
            return await DefaultSender.SendAsync(actMessasge);
        }
    }
}
