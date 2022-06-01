//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Janacek.Service
{
    /// <summary>
    /// Shared actor service used whenever you use shared actor.
    /// </summary>
    /// <seealso cref="T:Janacek.LocalActor"/>
    public class SharedActorService : LocalActor
    {
        /// <summary>
        /// The pattern senders.
        /// </summary>
        private PatternMatching<List<IChannelSender>> _patternSenders;

        /// <summary>
        /// The channel factory.
        /// </summary>
        private IChannelFactory _channelFactory;

        /// <summary>
        /// Options for controlling the operation.
        /// </summary>
        private SharedActorServiceOptions _options;

        /// <summary>
        /// Initializes a new instance of the Janacek.ActorService.SharedActorService class.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        /// <param name="configureOptions">Options for controlling the configure.</param>
        public SharedActorService(Action<SharedActorServiceOptions> configureOptions)
        {
            _patternSenders = new PatternMatching<List<IChannelSender>>();
            _channelFactory = new ChannelFactory();

            //configure options
            _options = new SharedActorServiceOptions();
            configureOptions(_options);
            if (_options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (_options.Sender == null || _options.Receiver == null) 
            {
                throw new ArgumentException("options", "You need to specify Receiver and Sender, or you have to specify ChannelHost and ChannelPort.");
            }

            IChannelSender sender = _options.Sender;
            IChannelReceiver receiver = _options.Receiver;

            ((SharedActorService)this).Add("role$:janacek,cmd$:add", OnAdd);
            ((SharedActorService)this).Add("role$:janacek,cmd$:act", OnAct);
            ((SharedActorService)this).Listen(receiver);
            ((SharedActorService)this).Client(sender);
        }

        /// <summary>
        /// Executes the add action.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// An asynchronous result that yields a Message.
        /// </returns>
        private Task<Message> OnAdd(Message message)
        {
            if (_options.DebugOutput)
            {
                Debug.WriteLine($"OnAdd: {message.Serialize()}");
            }
            
            var pattern = message["pattern$"].ToString();
            var channel = new Message(JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(
                message["channel$"].ToString()));
            var host = channel["host"].ToString();
            var sender = _channelFactory.CreateChannelSender(host, channel);
            var patternSenders = _patternSenders.Find(pattern);
            if (patternSenders == null)
            {
                _patternSenders.Add(pattern, new List<IChannelSender>()
                {
                    sender
                });
            }
            else
            {
                patternSenders.Value.Add(sender);
            }
            return Task.FromResult(Message.Empty);
        }

        /// <summary>
        /// Executes the act action.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// An asynchronous result that yields a Message.
        /// </returns>
        private async Task<Message> OnAct(Message message)
        {
            if (_options.DebugOutput)
            {
                Debug.WriteLine($"OnAct: {message.Serialize()}");
            }
            
            Message toSend = new Message(JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(
                message["data$"].ToString()));
            var patternSenders = _patternSenders.Match(toSend);
            if (patternSenders != null)
            {
                var tasks = new List<Task<Message>>(patternSenders.Count);
                foreach (IChannelSender patternSender in patternSenders)
                {
                    var task = patternSender.SendAsync(toSend);
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
                return tasks[0].Result;
            }
            else
            {
                // no listener for the message
                return Message.Empty;
            }
        }
    }
}
