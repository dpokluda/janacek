//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;

namespace Janacek
{
    public class CoordinatorClient : SharedActor
    {
        private new readonly CoordinatorClientOptions _options;

        public CoordinatorClient(Action<CoordinatorClientOptions> configureOptions)
            : base()
        {
            _options = new CoordinatorClientOptions();
            configureOptions(_options);
            if (_options == null)
            {
                throw new ArgumentNullException("options");
            }

            TreatAsCallRequiredBeforeBuild = true;
        }

        public override IActor Build(Action<SharedActorBuildOptions> configureOptions)
        {
            throw new NotSupportedException("Call Build without any option configuration action.");
        }

        public override IActor Build(Action<LocalActorBuildOptions> configureOptions)
        {
            throw new NotSupportedException("Call Build without any option configuration action.");
        }

        public override IActor Build()
        {
            if (PendingPatterns != null)
            { 
                base.Listen(new QueueChannelReceiver(
                    options =>
                    {
                        options.ConnectionString = _options.ConnectionString;
                        options.QueueName = _options.ServiceQueueName;
                        options.SleepBetweenMessages = _options.SleepBetweenMessages;
                    }));
            }

            return base.Build(
                options =>
                {
                    options.Sender = new QueueChannelSender(
                        co =>
                        {
                            co.ConnectionString = _options.ConnectionString;
                            co.QueueName = _options.CoordinatorQueueName;
                        }
                    );
                });
        }
    }
}
