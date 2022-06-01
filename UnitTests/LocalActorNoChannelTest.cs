//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Janacek;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class LocalActorNoChannelTest
    {
        private static readonly Random Generator = new Random((int)DateTime.UtcNow.Ticks);

        [TestMethod]
        public async Task Run()
        {
            var actor = new LocalActor()
                        .Add("cmd:sum", OnSum)
                        .TreatAs<LocalActor>()
                        .Build()
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

        [TestMethod]
        public async Task UnknownMessage()
        {
            var actor = new LocalActor()
                        .Add("cmd:product", OnSum)
                        .TreatAs<LocalActor>()
                        .Build()
                        .Run();
            Assert.IsNotNull(actor);

            try
            {
                await actor.ActAsync(new Message
                {
                    ["cmd"] = "sum",
                    ["x"] = 11,
                    ["y"] = 22
                });
                
                Assert.Fail("Unknown message error expected.");
            }
            catch (TransportationException)
            {
                // expected
            }
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
