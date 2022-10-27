//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Janacek;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UdpChannelTests
    {
        private static readonly Random Generator = new Random((int)DateTime.UtcNow.Ticks);
        private static string _received;

        [TestMethod]
        public async Task SendRequest()
        {
            // channel address
            string address = GetServiceAddress();

            // sender
            var sender = new UdpChannelSender(options => { options.Address = address; });

            // receiver
            var receiver = new UdpChannelReceiver(options => { options.Address = address; });
            var task = receiver.StartAsync(OnMessage);

            // send message
            _received = null;
            var message = new Message
            {
                ["request"] = "Hello world"
            };
            var response = await sender.SendAsync(message);

            await Task.Delay(50);

            // process result
            Assert.IsNotNull(response);
            Assert.AreEqual(message.Serialize(), _received);

            // stop server
            receiver.Stop();
        }

        [TestMethod]
        public async Task OnEmptyRequest()
        {
            // channel address
            string address = GetServiceAddress();

            // sender
            var sender = new UdpChannelSender(options => { options.Address = address; });

            // receiver
            var receiver = new UdpChannelReceiver(options => { options.Address = address; });
            var task = receiver.StartAsync(OnMessage);

            // send message
            try
            {
                await sender.SendAsync(null);
                Assert.Fail();
            }
            catch (Exception exception)
            {
                Assert.IsTrue(exception is ArgumentNullException);
            }

            // stop server
            receiver.Stop();
        }

        [TestMethod]
        public async Task CancelAfterOneRequest()
        {
            // channel address
            string address = GetServiceAddress();

            // sender
            var sender = new UdpChannelSender(options => { options.Address = address; });

            // receiver
            var receiver = new UdpChannelReceiver(
                options =>
                {
                    options.Address = address;
                    options.CheckCancellationTokenTimeSpan = TimeSpan.FromMilliseconds(10);
                });
            var source = new CancellationTokenSource();
            var task = receiver.StartAsync(OnMessage, source.Token);

            // send message
            _received = null;
            var message = new Message
            {
                ["request"] = "Hello world"
            };
            var response = await sender.SendAsync(message);

            await Task.Delay(50);

            // process result
            Assert.IsNotNull(response);
            Assert.AreEqual(message.Serialize(), _received);

            // cancel server 
            Assert.IsTrue(receiver.IsRunning);
            source.Cancel();
            await Task.Delay(50);
            Assert.IsFalse(receiver.IsRunning);
            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
        }

        [TestMethod]
        public void Stop()
        {
            // channel address
            string address = GetServiceAddress();

            // receiver
            var receiver = new UdpChannelReceiver(options => { options.Address = address; });
            var task = receiver.StartAsync(OnMessage);
            Assert.IsTrue(receiver.IsRunning);
            receiver.Stop();
            Assert.IsFalse(receiver.IsRunning);
        }

        private static Task<Message> OnMessage(Message msg, IChannelReceiver receiver)
        {
            if (msg != null)
            {
                _received = msg.Serialize();
            }

            return Task.FromResult<Message>(null);
        }

        private static string GetServiceAddress()
        {
            var port = Generator.Next(61000, 62000);
            return $"http://localhost:{port}/";
        }
    }
}
