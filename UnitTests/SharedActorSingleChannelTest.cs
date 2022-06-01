//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Janacek;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTests
{
    [TestClass]
    public class SharedActorSingleChannelTest
    {
        private static readonly Random Generator = new Random((int)DateTime.UtcNow.Ticks);

        private PatternMatching<List<IChannelSender>> _patternSenders;
        private IChannelFactory _channelFactory;

        [TestMethod]
        public async Task Run()
        {
            // setup shared janacek service
            var address = GetServiceAddress();
            _patternSenders = new PatternMatching<List<IChannelSender>>();
            _channelFactory = new ChannelFactory();

            var janacek = new LocalActor()
                          .Add("role$:janacek,cmd$:add", OnAdd)
                          .Add("role$:janacek,cmd$:act", OnAct)
                          .Listen(
                              new HttpChannelReceiver(
                                  options =>
                                  {
                                      options.Address = address;
                                      options.IncludeExceptionDetails = true;
                                  }))
                          .Client(
                              new HttpChannelSender(
                                  options =>
                                  {
                                      options.Address = address;
                                  }))

                          .Build()
                          .Run();

            var actor = new SharedActor()
                        .Add("cmd:sum", OnSum)
                        .Listen(new HttpChannelReceiver(
                            options =>
                            {
                                options.Address = GetServiceAddress();
                            }))
                        .TreatAs<SharedActor>()
                        .Build(
                            options =>
                            {
                                options.Sender = new HttpChannelSender(co => co.Address = address);
                            })
                        .Run();
            Assert.IsNotNull(actor);

            var response = await actor.ActAsync(new Message
            {
                ["cmd"] = "sum",
                ["x"] = 11,
                ["y"] = 22
            });

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ContainsKey("sum"));
            Assert.AreEqual(33, Convert.ToInt32(response["sum"]));
        }

        private Task<Message> OnSum(Message message)
        {
            return Task.FromResult(new Message
            {
                ["sum"] = Convert.ToInt32(message["x"]) +
                          Convert.ToInt32(message["y"])
            });
        }

        private Task<Message> OnAdd(Message message)
        {
            var pattern = message["pattern$"].ToString();
            var channel = new Message(JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(
                message["channel$"].ToString()));
            var host = channel["host"].ToString();
            var sender = _channelFactory.CreateChannelSender(host, channel);
            var patternSenders = _patternSenders.Find(pattern);
            if (patternSenders == null)
            {
                _patternSenders.Add(pattern, new List<IChannelSender>()
                {
                    sender
                });
            }
            return Task.FromResult(Message.Empty);
        }

        private async Task<Message> OnAct(Message message)
        {
            Message toSend = new Message(JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(
                message["data$"].ToString()));
            var patternSenders = _patternSenders.Match(toSend);
            if (patternSenders != null)
            {
                var tasks = new List<Task<Message>>(patternSenders.Count);
                foreach (IChannelSender patternSender in patternSenders)
                {
                    var task = patternSender.SendAsync(toSend);
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
                return tasks[0].Result;
            }
            else
            {
                // no listener for the message
                return Message.Empty;
            }
        }

        private static string GetServiceAddress()
        {
            var port = Generator.Next(61000, 62000);
            return $"http://localhost:{port}/";
        }
    }
}
