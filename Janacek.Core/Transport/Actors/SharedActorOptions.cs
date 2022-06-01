//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

namespace Janacek
{
    /// <summary>
    /// Shared actor build options.
    /// </summary>
    /// <seealso cref="T:Janacek.IBuildOptions"/>
    public class SharedActorBuildOptions : IBuildOptions
    {
        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        public IChannelSender Sender { get; set; }
    }

    /// <summary>
    /// Shared actor run options.
    /// </summary>
    /// <seealso cref="T:Janacek.IRunOptions"/>
    public class SharedActorRunOptions : IRunOptions
    { }
}
