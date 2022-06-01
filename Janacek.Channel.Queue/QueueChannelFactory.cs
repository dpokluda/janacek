//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

namespace Janacek
{
    public class QueueChannelFactory : IChannelFactory
    {
        static QueueChannelFactory()
        {
            ChannelFactory.RegisterChannelFactory("Queue", new QueueChannelFactory());
        }

        public IChannelSender CreateChannelSender(string host, Message serialized)
        {
            var queueName = serialized["queueName"].ToString();
            var connectionString = serialized["connectionString"].ToString();
            return new QueueChannelSender(options =>
            {
                options.ConnectionString = connectionString;
                options.QueueName = queueName;
            });
        }

        public IChannelReceiver CreateChannelReceiver(string host, Message serialized)
        {
            var queueName = serialized["queueName"].ToString();
            var connectionString = serialized["connectionString"].ToString();
            return new QueueChannelReceiver(options =>
            {
                options.ConnectionString = connectionString;
                options.QueueName = queueName;
            });
        }
    }
}
