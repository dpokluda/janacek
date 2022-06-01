//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;

namespace Janacek
{
    public class CoordinatorClientOptions
    {
        public string ConnectionString { get; set; }

        public string CoordinatorQueueName { get; set; }

        public string ServiceQueueName { get; set; }

        public TimeSpan SleepBetweenMessages { get; set; }
    }
}
