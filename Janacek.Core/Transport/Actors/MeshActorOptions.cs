namespace Janacek
{
    /// <summary>
    /// Mesh actor build options.
    /// </summary>
    /// <seealso cref="T:Janacek.IBuildOptions"/>
    public class MeshActorBuildOptions : IBuildOptions
    {
        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        public IChannelSender Sender { get; set; }
        
        public string LocalPort { get; set; }
        
        public string[] KnownServers { get; set; }
    }

    /// <summary>
    /// Mesh actor run options.
    /// </summary>
    /// <seealso cref="T:Janacek.IRunOptions"/>
    public class MeshActorRunOptions : IRunOptions
    { }
}