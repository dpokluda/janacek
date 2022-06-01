//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Janacek
{
    /// <summary>
    /// A local method call channel.
    /// </summary>
    /// <seealso cref="T:Janacek.IChannelReceiver"/>
    /// <seealso cref="T:Janacek.IChannelSender"/>
    public class LocalMethodCallChannel : IChannelReceiver, IChannelSender
    {
        private Func<Message, IChannelReceiver, Task<Message>> _localMethodCallFunctor;   

        /// <summary>
        /// Gets the channel host name (part of channel identity).
        /// </summary>
        /// <seealso cref="P:Janacek.IChannelReceiver.Host"/>
        public string Host
        {
            get
            {
                return "Local";
            }
        }

        /// <summary>
        /// Serialize the channel into a Message.
        /// </summary>
        /// <returns>
        /// Message containing channel data.
        /// </returns>
        /// <seealso cref="M:Janacek.IChannelReceiver.Serialize()"/>
        Message IChannelSender.Serialize()
        {
            return new Message
            {
                ["host"] = Host
            };
        }

        /// <summary>
        /// Sends an asynchronous message through the channel.
        /// </summary>
        /// <exception cref="TransportationException">Thrown when a Transportation error condition occurs.</exception>
        /// <param name="message">The message to send.</param>
        /// <returns>
        /// An asynchronous result with the response message.
        /// </returns>
        /// <seealso cref="M:Janacek.IChannelSender.SendAsync(Message)"/>
        public Task<Message> SendAsync(Message message)
        {
            if (IsRunning && _localMethodCallFunctor != null)
            {
                return _localMethodCallFunctor(message, this);
            }
            
            throw new TransportationException("Local channel is not running.");
        }

        /// <summary>
        /// Serialize the channel into a Message.
        /// </summary>
        /// <returns>
        /// Message containing channel data.
        /// </returns>
        /// <seealso cref="M:Janacek.IChannelReceiver.Serialize()"/>
        Message IChannelReceiver.Serialize()
        {
            return new Message
            {
                ["host"] = Host
            };
        }

        /// <summary>
        /// Gets a value indicating whether this channel receiver is currently listening for incoming messages.
        /// </summary>
        /// <value>
        /// <c>True</c> if the channel is running; otherwise, <c>false</c>.
        /// </value>
        /// <seealso cref="P:Janacek.IChannelReceiver.IsRunning"/>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Starts the channel receiver to listen for incoming messages.
        /// </summary>
        /// <param name="onReceive">Functor to be executed whenever a message is received.</param>
        /// <param name="token">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        /// <seealso cref="M:Janacek.IChannelReceiver.StartAsync(Func{Message,IChannelReceiver,Task{Message}},CancellationToken)"/>
        public Task StartAsync(Func<Message, IChannelReceiver, Task<Message>> onReceive, CancellationToken token = default(CancellationToken))
        {
            _localMethodCallFunctor = onReceive;
            IsRunning = true;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Stops this channel receiver to listen for incoming messages.
        /// </summary>
        /// <seealso cref="M:Janacek.IChannelReceiver.Stop()"/>
        public void Stop()
        {
            IsRunning = false;
        }
    }
}