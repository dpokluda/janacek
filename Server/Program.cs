using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Janacek;
using Newtonsoft.Json;

namespace Server
{
    class Program
    {
        public static string JanacekServiceAddress = "http://localhost:8100/";

        private static Dictionary<string, IChannelSender> _channels = new Dictionary<string, IChannelSender>();
        private static Dictionary<string, PatternMatching<bool>> _patterns = new Dictionary<string, PatternMatching<bool>>();

        private static IMessageProducer _telemetry;

        static void Main(string[] args)
        {
            Console.WriteLine("******************************************************************************");
            Console.WriteLine("*                                                                            *");
            Console.WriteLine("* JanacekServer - pattern matching and transport independence library        *");
            Console.WriteLine("*                                                                            *");
            Console.WriteLine("******************************************************************************");
            Console.WriteLine("Running...");

            // run async forever
            RunAsync()
                .GetAwaiter()
                .GetResult();
        }

        private static async Task RunAsync()
        {
            // send telemetry messages to itself for further routing to subscribers
            _telemetry = new JanacekProducer();

            // main consumer subscribing to all "janacek" messages
            var main = new MessageConsumer()
                       .Add("role:janacek,cmd:add", OnAddMessage)
                       .Add("role:janacek,cmd:act", OnActMessage)
                       .Listen(new HttpChannelReceiver(
                           options =>
                           {
                               options.ServiceAddress = JanacekServiceAddress;
                           }))
                       .Run();

            // wait forever
            await main;
        }


        private static async Task<Message> OnAddMessage(Message msg)
        {
            await FireTelemetryEvent("add-started", msg);

            // parse add message parameters
            string pattern = msg["pattern"] as string;
            string channelName = msg["channel-name"] as string;
            string channelAddress = msg["channel-address"] as string;

            var channelInfo = $"{channelName};{channelAddress}";
            if (!_patterns.ContainsKey(channelInfo))
            {
                // create proper channel sender
                IChannelSender channel = CreateChannelSender(channelInfo);

                // register pattern and assign it to the channel sender
                _channels.Add(channelInfo, channel);
                var patterns = new PatternMatching<bool>();
                patterns.Add(pattern, true);
                _patterns.Add(channelInfo, patterns);
            }
            else
            {
                // register pattern and assign it to the channel sender
                _patterns[channelInfo].Add(pattern, true);
            }

            // acknowledge add operation completed
            return new Message
            {
                ["role"] = "janacek",
                ["cmd"] ="ack"
            };
        }

        private static async Task<Message> OnActMessage(Message msg)
        {
            // extract the actual ACT message
            Message toSend = new Message(JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(msg["data"].ToString()));
            await FireTelemetryEvent("act-started", toSend);

            // route the message to the right channel(s)
            Message result = await RouteAndSendMessageInternal(toSend);
            await FireTelemetryEvent("act-finished", result);

            // finish
            return result;
        }

        private static async Task<Message> RouteAndSendMessageInternal(Message msg)
        {
            Message result = null;
            // route the message - find the proper channel
            foreach ((string key, PatternMatching<bool> patterns) in _patterns)
            {
                if (patterns.Match(msg))
                {
                    var channel = _channels[key];

                    // send the message to the right channel
                    var response = await channel.SendAsync(msg);

                    // keep the first response
                    if (result == null)
                    {
                        result = response;
                    }
                }
            }

            return result;
        }

        private static async Task FireTelemetryEvent(string type, Message data)
        {
            // telemetry event
            var telemetryMsg = new Message
            {
                ["role"] = "telemetry",
                ["event"] = type,
            };
            if (data != null)
            {
                telemetryMsg.Add("data", data);
            }

            // route the message to the right channel(s)
            await RouteAndSendMessageInternal(telemetryMsg);
        }

        private static IChannelSender CreateChannelSender(string channelInfo)
        {
            var parts = channelInfo.Split(new[] { ';' });
            if (parts[0] == "Http")
            {
                return new HttpChannelSender(
                    options =>
                    {
                        options.ServiceAddress = parts[1];
                    });
            }
            else
            {
                return new QueueChannelSender(
                    options =>
                    {
                        options.QueueName = parts[1];
                        options.ConnectionString = "UseDevelopmentStorage=true;";
                    });
            }
        }
    }
}
