//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Janacek
{
    public class ChannelFactory : IChannelFactory
    {
        private static Dictionary<string, IChannelFactory> ChannelFactories = new Dictionary<string, IChannelFactory>();

        public static void RegisterChannelFactory(string host, IChannelFactory factory)
        {
            if (ChannelFactories.ContainsKey(host))
            {
                throw new NotSupportedException("Channel factory for the same host is already registered.");
            }

            ChannelFactories.Add(host, factory);
        }

        public virtual IChannelSender CreateChannelSender(string host, Message serialized)
        {
            if (host == "Http")
            {
                var port = serialized["port"].ToString();
                return new HttpChannelSender(options => options.Address = port);
            }
            else if (ChannelFactories.ContainsKey(host))
            {
                return ChannelFactories[host].CreateChannelSender(host, serialized);
            }
            else
            {
                throw new TransportationException("Unknown channel host.");
            }
        }

        public IChannelReceiver CreateChannelReceiver(string host, Message serialized)
        {
            if (host == "Http")
            {
                var port = serialized["port"].ToString();
                return new HttpChannelReceiver(options => options.Address = port);
            }
            else if (ChannelFactories.ContainsKey(host))
            {
                return ChannelFactories[host].CreateChannelReceiver(host, serialized);
            }
            else
            {
                throw new TransportationException("Unknown channel host.");
            }
        }
    }
}
