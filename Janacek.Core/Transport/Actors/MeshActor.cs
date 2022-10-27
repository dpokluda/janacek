using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Janacek
{
    public class MeshActor : LocalActor
    {
        protected MeshActorBuildOptions _options;
        protected ChannelFactory _channelFactory;

        protected UdpChannelReceiver _meshReceiver;
        public MeshActor()
            : base()
        {
            TreatAsCallRequiredBeforeBuild = true;
            _channelFactory = new ChannelFactory();
        }        
        
                /// <summary>
        /// Build the pipeline by properly connecting all channels together.
        /// </summary>
        /// <param name="configureOptions">(Optional) Options configuration.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Build()"/>
        public virtual IActor Build(Action<MeshActorBuildOptions> configureOptions)
        {
            // configure options
            _options = new MeshActorBuildOptions();
            configureOptions(_options);
            if (_options == null)
            {
                throw new ArgumentNullException("options");
            }

            _meshReceiver = new UdpChannelReceiver(options =>
            {
                options.Address = _options.LocalPort;
                
            });
            _meshReceiver.StartAsync(OnMeshReceive);
            
            if (_options.KnownServers != null && _options.KnownServers.Length > 0)
            {
                foreach (string knownServer in _options.KnownServers)
                {
                    var knownServerSender = new UdpChannelSender(options =>
                    {
                        options.Address = knownServer;
                        
                    });
                    foreach ((IChannelReceiver channelReceiver, PatternMatching<Func<Message, Task<Message>>> patterns) in PatternReceivers)
                    {
                        foreach (PatternValuePair<Func<Message, Task<Message>>> patternValuePair in patterns.Patterns)
                        {
                            knownServerSender.SendAsync(new Message
                                         {
                                             ["role$"] = "janacek.mesh",
                                             ["cmd$"] = "add",
                                             ["pattern$"] = patternValuePair.Pattern.ToString(),
                                             ["channel$"] = channelReceiver.Serialize(),
                                             ["mesh$"] = _options.LocalPort,
                                         })
                                         .GetAwaiter()
                                         .GetResult();
                        }
                    }
                }
            }

            return this;
        }

        private Task<Message> OnMeshReceive(Message message, IChannelReceiver receiver)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Build the pipeline by properly connecting all channels together.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="configureOptions">Options configuration.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.LocalActor.Build(Action{LocalActorBuildOptions})"/>
        public override IActor Build(Action<LocalActorBuildOptions> configureOptions)
        {
            throw new NotSupportedException("Please call Build method with MeshActorBuildOptions configuration.");
        }

        /// <summary>
        /// Build the pipeline by properly connecting all channels together.
        /// </summary>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Build()"/>
        public override IActor Build()
        {
            throw new NotSupportedException("Please call Build method with MeshActorBuildOptions configuration.");
        }

        /// <summary>
        /// Start the pipeline by starting all channel receivers for example.
        /// </summary>
        /// <exception cref="TransportationException">Thrown when a transportation error condition occurs.</exception>
        /// <param name="configureOptions">(Optional) Options configuration.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Run()"/>
        public virtual IActor Run(Action<SharedActorRunOptions> configureOptions)
        {
            if (PendingPatterns != null)
            {
                throw new TransportationException("There are patterns without listening channel.");
            }

            var tasks = new List<Task>();
            foreach (IChannelReceiver receiver in PatternReceivers.Keys)
            {
                var task = receiver.StartAsync(OnDefaultReceiverMessage);
                tasks.Add(task);
            }

            return this;
        }

        /// <summary>
        /// Start the pipeline by starting all channel receivers for example.
        /// </summary>
        /// <param name="configureOptions">Options configuration.</param>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.LocalActor.Run(Action{LocalActorRunOptions})"/>
        public override IActor Run(Action<LocalActorRunOptions> configureOptions)
        {
            return base.Run(null);
        }

        /// <summary>
        /// Start the pipeline by starting all channel receivers for example.
        /// </summary>
        /// <exception cref="TransportationException">Thrown when a transportation error condition occurs.</exception>
        /// <returns>
        /// Transportation actor.
        /// </returns>
        /// <seealso cref="M:Janacek.IActor.Run()"/>
        public new virtual IActor Run()
        {
            return Run(null);
        }
    }
}