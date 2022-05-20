using System.Threading.Tasks;

namespace Janacek
{
    public class MessageProducer : IMessageProducer
    {
        protected readonly PatternMatching<IChannelSender> PatternSenders;
        protected IChannelSender DefaultSender;

        public MessageProducer()
        {
            PatternSenders = new PatternMatching<IChannelSender>();
            DefaultSender = null;
        }

        public virtual IMessageProducer Client(IChannelSender sender)
        {
            DefaultSender = sender;
            return this;
        }

        public virtual IMessageProducer Client(IChannelSender sender, string pattern)
        {
            PatternSenders.Add(pattern, sender);
            return this;
        }

        public virtual async Task<Message> ActAsync(Message message)
        {
            var sender = PatternSenders.Match(message);
            if (sender == null)
            {
                sender = DefaultSender;
            }

            if (sender == null)
            {
                throw new TransportationException("No sender available for the message.");
            }

            return await sender.SendAsync(message);
        }
    }
}
