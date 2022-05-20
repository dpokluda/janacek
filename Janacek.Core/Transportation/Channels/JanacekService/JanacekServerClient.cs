using System;

namespace Janacek
{
    /// <summary>
    /// Janacek service client component used to route messages through the Janacek service.
    /// This is a helper class that hides the details about channel used by the service.
    /// </summary>
    /// <seealso cref="T:Janacek.HttpChannelSender"/>
    public class JanacekServerClient : HttpChannelSender
    {
        /// <summary>
        /// The Janacek service address.
        /// </summary>
        private const string JanacekServerAddress = "http://localhost:8100/";

        /// <summary>
        /// Initializes a new instance of the JanacekServerClient class.
        /// </summary>
        public JanacekServerClient()
            : base(options => { options.ServiceAddress = JanacekServerAddress; })
        { }

        /// <summary>
        /// Initializes a new instance of the JanacekServerClient class.
        /// </summary>
        /// <param name="service">The service.</param>
        public JanacekServerClient(string service)
            : base(options => { options.ServiceAddress = service; })
        { }

        /// <summary>
        /// Initializes a new instance of the JanacekServerClient class.
        /// </summary>
        /// <param name="configureOptions">Options for controlling the configure.</param>
        public JanacekServerClient(Action<HttpChannelSenderOptions> configureOptions)
            : base(configureOptions)
        { }
    }
}
