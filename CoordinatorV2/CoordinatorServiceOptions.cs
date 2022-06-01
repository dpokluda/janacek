//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;

namespace Janacek.Service
{
    public class CoordinatorServiceOptions
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }

        public TimeSpan SleepBetweenMessages { get; set; }
    }
}
