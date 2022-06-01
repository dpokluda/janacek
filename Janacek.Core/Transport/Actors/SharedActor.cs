//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Janacek
{
    /// <summary>
    /// Shared transportation actor.
    /// </summary>
    /// <seealso cref="T:Janacek.IActor"/>
    public class SharedActor : LocalActor
    {
        protected SharedActorBuildOptions _options;
        protected ChannelFactory _channelFactory;

        /// <summary>
        /// Initializes a new instance of the Janacek.LocalActor class.
        /// </summary>
        public SharedActor()
            : base()
        {
            TreatAsCallRequiredBeforeBuild = true;
            _channelFactory = new ChannelFactory();
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
        public override async Task<Message> ActAsync(Message message)
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

            var actMessasge = new Message
            {
                ["role$"] = "janacek",
                ["cmd$"] = "act",
                ["data$"] = message
            };

            return await sender.SendAsync(actMessasge);
        }

        /// <summary>
        /// Build the pipeline by properly connecting all channels together.
        /// </summary>
        /// <param name="configureOptions">(Optional) Options configuration.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Build()"/>
        public virtual IActor Build(Action<SharedActorBuildOptions> configureOptions)
        {
            // configure options
            _options = new SharedActorBuildOptions();
            configureOptions(_options);
            if (_options == null)
            {
                throw new ArgumentNullException("options");
            }

            DefaultSender = _options.Sender;

            foreach ((IChannelReceiver channelReceiver, PatternMatching<Func<Message, Task<Message>>> patterns) in PatternReceivers)
            {
                foreach (PatternValuePair<Func<Message, Task<Message>>> patternValuePair in patterns.Patterns)
                {
                    DefaultSender.SendAsync(new Message
                        {
                            ["role$"] = "janacek",
                            ["cmd$"] = "add",
                            ["pattern$"] = patternValuePair.Pattern.ToString(),
                            ["channel$"] = channelReceiver.Serialize()
                        })
                        .GetAwaiter()
                        .GetResult();
                }
            }

            return this;
        }

        /// <summary>
        /// Build the pipeline by properly connecting all channels together.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="configureOptions">Options configuration.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.LocalActor.Build(Action{LocalActorBuildOptions})"/>
        public override IActor Build(Action<LocalActorBuildOptions> configureOptions)
        {
            throw new NotSupportedException("Please call Build method with SharedActorBuildOptions configuration.");
        }

        /// <summary>
        /// Build the pipeline by properly connecting all channels together.
        /// </summary>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Build()"/>
        public override IActor Build()
        {
            throw new NotSupportedException("Please call Build method with SharedActorBuildOptions configuration.");
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
        public virtual IActor Run(Action<SharedActorRunOptions> configureOptions)
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
        /// <param name="configureOptions">Options configuration.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.LocalActor.Run(Action{LocalActorRunOptions})"/>
        public override IActor Run(Action<LocalActorRunOptions> configureOptions)
        {
            return base.Run(null);
        }

        /// <summary>
        /// Start the pipeline by starting all channel receivers for example.
        /// </summary>
        /// <exception cref="TransportationException">Thrown when a transportation error condition occurs.</exception>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Run()"/>
        public new virtual IActor Run()
        {
            return Run(null);
        }
    }
}
