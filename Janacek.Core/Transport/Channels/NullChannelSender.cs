//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System.Threading.Tasks;

namespace Janacek
{
    public class NullChannelSender : IChannelSender
    {
        public string Host {
            get
            {
                return "Null";
            }
        }

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

        public Task<Message> SendAsync(Message message)
        {
            return Task.FromResult(Message.Empty);
        }
    }
}
