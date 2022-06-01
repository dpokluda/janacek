//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Janacek
{
    public class NullChannelReceiver : IChannelReceiver
    {
        public string Host
        {
            get
            {
                return "Null";
            }
        }

        public bool IsRunning { get; private set; }

        /// <summary>
        /// Serialize the channel into a Message.
        /// </summary>
        /// <returns>
        /// Message containing channel data.
        /// </returns>
        /// <seealso cref="M:Janacek.IChannelReceiver.Serialize()"/>
        public Message Serialize()
        {
            return new Message
            {
                ["host"] = Host
            };
        }

        public Task StartAsync(Func<Message, IChannelReceiver, Task<Message>> onReceive, CancellationToken token = default(CancellationToken))
        {
            IsRunning = true;
            return Task.FromResult(0);
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }
}
