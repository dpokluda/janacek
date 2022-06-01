//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Janacek;
using Janacek.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class SharedActorWithServiceTest
    {
        private static readonly Random Generator = new Random((int)DateTime.UtcNow.Ticks);

        [TestMethod]
        public async Task Run()
        {
            // setup shared janacek service
            var address = GetServiceAddress();
            var service = new SharedActorService(
                           options =>
                           {
                               options.Sender = new HttpChannelSender(co => co.Address = address);
                               options.Receiver = new HttpChannelReceiver(co => co.Address = address);
                           })
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
                                options.Sender = new HttpChannelSender(so => so.Address = address);
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

        private static string GetServiceAddress()
        {
            var port = Generator.Next(61000, 62000);
            return $"http://localhost:{port}/";
        }
    }
}
