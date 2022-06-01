//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

namespace Janacek
{
    public interface IChannelFactory
    {
        IChannelSender CreateChannelSender(string host, Message serialized);

        IChannelReceiver CreateChannelReceiver(string host, Message serialized);
    }
}
