using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Janacek;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class HttpChannelTests
    {
        private static Random generator = new Random((int)DateTime.UtcNow.Ticks);

        [TestMethod]
        public async Task SendRequest()
        {
            // channel address
            string service = GetServiceAddress();

            // sender
            var sender = new HttpChannelSender(options => { options.ServiceAddress = service; });

            // receiver
            var receiver = new HttpChannelReceiver(options => { options.ServiceAddress = service; });
            var task = receiver.StartAsync(OnMessage);

            // send message
            var response = await sender.SendAsync(
                new Message
                {
                    ["request"] = "Hello world"
                });

            // process result
            Assert.IsNotNull(response);
            Assert.IsTrue(response.ContainsKey("response"));
            Assert.AreEqual(":-)", response["response"]);

            // stop server
            receiver.Stop();
        }

        [TestMethod]
        public async Task OnUnknownRequest()
        {
            // channel address
            string service = GetServiceAddress();

            // receiver
            var receiver = new HttpChannelReceiver(options => { options.ServiceAddress = service; });
            var task = receiver.StartAsync(OnMessage);

            // send something else than Message
            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, service);
            requestMessage.Content = new StringContent("?", Encoding.UTF8); 
            var response = await client.SendAsync(requestMessage);

            // process result
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            // stop server
            receiver.Stop();
        }

        [TestMethod]
        public async Task OnEmptyRequest()
        {
            // channel address
            string service = GetServiceAddress();

            // receiver
            var receiver = new HttpChannelReceiver(options => { options.ServiceAddress = service; });
            var task = receiver.StartAsync(OnMessage);

            // send no content
            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, service);
            var response = await client.SendAsync(requestMessage);

            // process result
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            // stop server
            receiver.Stop();
        }

        [TestMethod]
        public async Task CancelAfterOneRequest()
        {
            // channel address
            string service = GetServiceAddress();

            // sender
            var sender = new HttpChannelSender(options => { options.ServiceAddress = service; });

            // receiver
            var receiver = new HttpChannelReceiver(
                options =>
                {
                    options.ServiceAddress = service;
                    options.CheckCancellationTokenTimeSpan = TimeSpan.FromMilliseconds(10);
                });
            var source = new CancellationTokenSource();
            var task = receiver.StartAsync(OnMessage, source.Token);

            // send message
            var response = await sender.SendAsync(
                               new Message
                               {
                                   ["request"] = "Hello world"
                               });

            // process result
            Assert.IsNotNull(response);
            Assert.IsTrue(response.ContainsKey("response"));
            Assert.AreEqual(":-)", response["response"]);

            // cancel server 
            Assert.IsTrue(receiver.IsRunning);
            source.Cancel();
            await Task.Delay(50);
            Assert.IsFalse(receiver.IsRunning);
            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
        }

        [TestMethod]
        public async Task Stop()
        {
            // channel address
            string service = GetServiceAddress();

            // receiver
            var receiver = new HttpChannelReceiver(options => { options.ServiceAddress = service; });
            var task = receiver.StartAsync(OnMessage);

            Assert.IsTrue(receiver.IsRunning);
            receiver.Stop();
            Assert.IsFalse(receiver.IsRunning);

            try
            {
                var sender = new HttpChannelSender(options =>
                {
                    options.ServiceAddress = service;
                    options.Timeout = TimeSpan.FromMilliseconds(100);
                });
                var response = await sender.SendAsync(
                                   new Message
                                   {
                                       ["request"] = "Hello world"
                                   });
                Assert.Fail("Server shouldn't be responding after Stop is called.");
            }
            catch (Exception ex)
            {
                if (ex is AssertFailedException)
                {
                    throw;
                }
            }
        }


        private static Task<Message> OnMessage(Message msg, IChannelReceiver receiver)
        {
            return Task.FromResult(new Message
            {
                ["response"] = ":-)"
            });
        }

        private static string GetServiceAddress()
        {
            var port = generator.Next(61000, 62000);
            return $"http://localhost:{port}/";
        }
    }
}
