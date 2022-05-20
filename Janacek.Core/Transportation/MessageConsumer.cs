using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Janacek
{
    public class MessageConsumer : IMessageConsumer
    {
        protected readonly PatternMatching<IChannelSender> PatternSenders;
        protected IChannelSender DefaultSender;

        private readonly Dictionary<IChannelReceiver, PatternMatching<Func<Message, Task<Message>>>> _receiverPatterns;
        protected PatternMatching<Func<Message, Task<Message>>> _pendingPatterns;

        public MessageConsumer()
        {
            PatternSenders = new PatternMatching<IChannelSender>();
            DefaultSender = null;

            _receiverPatterns = new Dictionary<IChannelReceiver, PatternMatching<Func<Message, Task<Message>>>>();
            _pendingPatterns = null;
        }

        public virtual IMessageConsumer Client(IChannelSender sender)
        {
            DefaultSender = sender;
            return this;
        }

        public virtual IMessageConsumer Client(IChannelSender sender, string pattern)
        {
            PatternSenders.Add(pattern, sender);
            return this;
        }

        public virtual IMessageConsumer Add(string pattern, Func<Message, Task<Message>> action)
        {
            if (_pendingPatterns == null)
            {
                _pendingPatterns = new PatternMatching<Func<Message, Task<Message>>>();
            }

            _pendingPatterns.Add(pattern, action);

            return this;
        }

        public virtual IMessageConsumer Listen(IChannelReceiver receiver)
        {
            if (_pendingPatterns == null)
            {
                throw new TransactionException("There are no patterns for this channel to listen to.");
            }
            if (_receiverPatterns.ContainsKey(receiver))
            {
                throw new TransactionException("This channel is already configured with patterns to listen to.");
            }

            _receiverPatterns.Add(receiver, _pendingPatterns);
            _pendingPatterns = null;

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

        public virtual Task Run()
        {
            if (_pendingPatterns != null)
            {
                throw new TransactionException("There are patterns without listening channel.");
            }

            var tasks = new List<Task>();
            foreach (IChannelReceiver receiver in _receiverPatterns.Keys)
            {
                var task = receiver.StartAsync(OnDefaultReceiverMessage);
                tasks.Add(task);
            }

            return Task.WhenAll(tasks);
        }

        protected async Task<Message> OnDefaultReceiverMessage(Message msg, IChannelReceiver receiver)
        {
            if (_receiverPatterns.ContainsKey(receiver))
            {
                Func<Message, Task<Message>> action = _receiverPatterns[receiver].Match(msg);
                return await action(msg);
            }
            else
            {
                // let's enumerate all patterns?
                throw new TransactionException("Unknown channel receiver.");
            }
        }
    }
}
