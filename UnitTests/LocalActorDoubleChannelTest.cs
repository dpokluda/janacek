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
    public class LocalActorDoubleChannelTest
    {
        private static readonly Random Generator = new Random((int)DateTime.UtcNow.Ticks);

        private IActor _actor = null;
        private bool _doubleChannelTestPassed = false;

        [TestMethod]
        public async Task Run()
        {
            _doubleChannelTestPassed = false;

            var requestAddress = GetServiceAddress();
            var responseAddress = GetServiceAddress();
            _actor = new LocalActor()
                        .Add("cmd:product", OnProduct)
                        .Listen(
                            new HttpChannelReceiver(
                                options =>
                                {
                                    options.Address = requestAddress;
                                    options.IncludeExceptionDetails = true;
                                }))
                        .Client(
                            new HttpChannelSender(
                                options =>
                                {
                                    options.Address = requestAddress;
                                }),
                            "cmd:product")
                        .Add("cmd:result", OnResult)
                        .Listen(
                            new HttpChannelReceiver(
                                options =>
                                {
                                    options.Address = responseAddress;
                                    options.IncludeExceptionDetails = true;
                                }))
                        .Client(
                            new HttpChannelSender(
                                options =>
                                {
                                    options.Address = responseAddress;
                                }),
                            "cmd:result")
                        .Build()
                        .Run();
            Assert.IsNotNull(_actor);

            Assert.IsFalse(_doubleChannelTestPassed);
            var response = await _actor.ActAsync(new Message
            {
                ["cmd"] = "product",
                ["x"] = 11,
                ["y"] = 22
            });
            Assert.IsNotNull(response);
            Assert.IsTrue(_doubleChannelTestPassed);

            // cleanup
            _actor = null;
        }

        private async Task<Message> OnProduct(Message message)
        {
            await _actor.ActAsync(new Message
            {
                ["cmd"] = "result",
                ["product"] = Convert.ToInt32(message["x"]) *
                              Convert.ToInt32(message["y"])
            });
            return Message.Empty;
        }

        private Task<Message> OnResult(Message message)
        {
            var product = Convert.ToInt32(message["product"]);
            Assert.AreEqual(242, Convert.ToInt32(product));
            _doubleChannelTestPassed = true;
            return Task.FromResult(Message.Empty);
        }

        private static string GetServiceAddress()
        {
            var port = Generator.Next(61000, 62000);
            return $"http://localhost:{port}/";
        }
    }
}
