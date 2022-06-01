//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Janacek
{
    /// <summary>
    /// Local transportation actor.
    /// </summary>
    /// <seealso cref="T:Janacek.IActor"/>
    public class LocalActor : IActor
    {
        /// <summary>
        /// Pattern senders.
        /// </summary>
        protected readonly PatternMatching<IChannelSender> PatternSenders;

        /// <summary>
        /// Default sender - used for all messages that don't match any pattern sender
        /// </summary>
        protected IChannelSender DefaultSender;

        /// <summary>
        /// Pattern receivers.
        /// </summary>
        protected readonly Dictionary<IChannelReceiver, PatternMatching<Func<Message, Task<Message>>>> PatternReceivers;

        /// <summary>
        /// Pending patterns.
        /// </summary>
        protected PatternMatching<Func<Message, Task<Message>>> PendingPatterns;

        /// <summary>
        /// True to treat as call required before build.
        /// </summary>
        protected bool TreatAsCallRequiredBeforeBuild = false;

        /// <summary>
        /// Initializes a new instance of the Janacek.LocalActor class.
        /// </summary>
        public LocalActor()
        {
            PatternSenders = new PatternMatching<IChannelSender>();
            DefaultSender = null;

            PatternReceivers = new Dictionary<IChannelReceiver, PatternMatching<Func<Message, Task<Message>>>>();
        }

        /// <summary>
        /// Register a client to send messages through. This method registers a default sender in case 
        /// there is no explicit pattern sender.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <exception cref="TransportationException">Thrown when a transportation error condition occurs.</exception>
        /// <param name="sender">Sender channel.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Client(IChannelSender)"/>
        public virtual IActor Client(IChannelSender sender)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (DefaultSender != null)
            {
                throw new TransportationException("There already is a default sender. You can only configure one default sender per actor instance.");
            }

            DefaultSender = sender;

            return this;
        }

        /// <summary>
        /// Register a client to send messages through. This method registers a sender for a given message pattern.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="sender">Sender channel.</param>
        /// <param name="pattern">Message pattern.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Client(IChannelSender,string)"/>
        public virtual IActor Client(IChannelSender sender, string pattern)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            PatternSenders.Add(pattern, sender);

            return this;
        }

        /// <summary>
        /// Send message.
        /// </summary>
        /// <exception cref="TransportationException">Thrown when a transportation error condition occurs.</exception>
        /// <param name="message">Message to send.</param>
        /// <returns>
        /// An asynchronous result that yields the response message (in case of a sync channel).
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.ActAsync(Message)"/>
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

        /// <summary>
        /// Map receiver/listener pattern to a local function. This is just adding new pattern, function pair 
        /// to the collection of pending maps. You still have to properly assign the
        /// pending map to the right receiver/listener channel.
        /// </summary>
        /// <param name="pattern">Request message pattern.</param>
        /// <param name="syncAction">Local function to process the request message.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Add(string,Func{Message,Task{Message}})"/>
        public virtual IActor Add(string pattern, Func<Message, Task<Message>> syncAction)
        {
            if (PendingPatterns == null)
            {
                PendingPatterns = new PatternMatching<Func<Message, Task<Message>>>();
            }

            PendingPatterns.Add(pattern, syncAction);

            return this;
        }

        /// <summary>
        /// Assign the pending pattern, function pairs to the specified receiver/listener channel. 
        /// This also resets pending patterns.
        /// </summary>
        /// <exception cref="TransportationException">Thrown when a transportation error condition occurs.</exception>
        /// <param name="receiver">Message channel receiver.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Listen(IChannelReceiver)"/>
        public virtual IActor Listen(IChannelReceiver receiver)
        {
            if (PendingPatterns == null)
            {
                throw new TransportationException("There are no patterns for this channel to listen to.");
            }
            if (PatternReceivers.ContainsKey(receiver))
            {
                throw new TransportationException("This channel is already configured with patterns to listen to.");
            }

            PatternReceivers.Add(receiver, PendingPatterns);
            PendingPatterns = null;

            return this;
        }

        /// <summary>
        /// Reinterpret transportation actor as a concrete transportation actor. 
        /// This is a helper method for building readable method chains.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <typeparam name="T">Concrete transportation actor type.</typeparam>
        /// <returns>
        /// Concrete implementation of transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.TreatAs{T}()"/>
        public T TreatAs<T>() where T : IActor
        {
            return (T)Convert.ChangeType(this, typeof(T));
        }

        /// <summary>
        /// Build the pipeline by properly connecting all channels together.
        /// </summary>
        /// <param name="configureOptions">(Optional) Options configuration.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Build()"/>
        public virtual IActor Build(Action<LocalActorBuildOptions> configureOptions)
        {
            if (TreatAsCallRequiredBeforeBuild)
            {
                throw new NotSupportedException("Please call TreatAs before calling Build method.");
            }

            if (PendingPatterns != null && PendingPatterns.Patterns.Count > 0)
            {
                var localChannel = new LocalMethodCallChannel();
                foreach (PatternValuePair<Func<Message,Task<Message>>> pattern in PendingPatterns.Patterns)
                {
                    PatternSenders.Add(pattern.Pattern.ToString(), localChannel);
                }
                PatternReceivers.Add(localChannel, PendingPatterns);
                PendingPatterns = null;
            }
            
            return this;
        }

        /// <summary>
        /// Build the pipeline by properly connecting all channels together.
        /// </summary>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Build()"/>
        public virtual IActor Build()
        {
            return Build(null);
        }

        /// <summary>
        /// Start the pipeline by starting all channel receivers for example.
        /// </summary>
        /// <exception cref="TransportationException">Thrown when a transportation error condition occurs.</exception>
        /// <param name="configureOptions">(Optional) Options configuration.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Run()"/>
        public virtual IActor Run(Action<LocalActorRunOptions> configureOptions)
        {
            if (PendingPatterns != null)
            {
                throw new TransportationException("There are patterns without listening channel.");
            }

            var tasks = new List<Task>();
            foreach (IChannelReceiver receiver in PatternReceivers.Keys)
            {
                var task = receiver.StartAsync(OnDefaultReceiverMessage);
                tasks.Add(task);
            }

            return this;
        }

        /// <summary>
        /// Start the pipeline by starting all channel receivers for example.
        /// </summary>
        /// <exception cref="TransportationException">Thrown when a transportation error condition occurs.</exception>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Run()"/>
        public virtual IActor Run()
        {
            return Run(null);
        }

        /// <summary>
        /// Executes the default receiver message action.
        /// </summary>
        /// <exception cref="TransportationException">Thrown when a transportation error condition occurs.</exception>
        /// <param name="message">The message.</param>
        /// <param name="receiver">The receiver.</param>
        /// <returns>
        /// An asynchronous result that yields a Message.
        /// </returns>
        protected async Task<Message> OnDefaultReceiverMessage(Message message, IChannelReceiver receiver)
        {
            if (PatternReceivers.ContainsKey(receiver))
            {
                Func<Message, Task<Message>> action = PatternReceivers[receiver].Match(message);
                if (action != null)
                {
                    return await action(message);
                }
            }
            
            throw new TransactionException("Pattern matching error: There is no receiver for the message.");
        }
    }
}
