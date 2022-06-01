//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;

namespace Janacek.Service
{
    public class CoordinatorService : SharedActorService
    {
        private CoordinatorServiceOptions _options;

        public CoordinatorService(Action<CoordinatorServiceOptions> configureOptions)
            : base(
                options =>
                {
                    var co = new CoordinatorServiceOptions();
                    configureOptions(co);

                    options.Sender = new NullChannelSender();
                    options.Receiver = new QueueChannelReceiver(
                            qo =>
                            {
                                qo.ConnectionString = co.ConnectionString;
                                qo.QueueName = co.QueueName;
                                qo.SleepBetweenMessages = co.SleepBetweenMessages;
                            });
                })
        {
            _options = new CoordinatorServiceOptions();
            configureOptions(_options);
            if (_options == null)
            {
                throw new ArgumentNullException("options");
            }
        }
    }
}
