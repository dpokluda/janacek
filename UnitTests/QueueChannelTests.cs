//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Janacek;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace UnitTests
{
    [TestClass]
    public class QueueChannelTests
    {
        private const string ConnectionString = "UseDevelopmentStorage=true;";

        private static readonly Random Generator = new Random((int)DateTime.UtcNow.Ticks);
        private static readonly ConcurrentDictionary<string, Message> Responses = new ConcurrentDictionary<string, Message>();

        private readonly CloudQueueClient _queueClient;

        public QueueChannelTests()
        {
            var storageAccount = CloudStorageAccount.Parse(ConnectionString);
            _queueClient = storageAccount.CreateCloudQueueClient();
        }

        [TestMethod]
        public async Task SendRequest()
        {
            // queue setup
            string queueName = GetQueueName();
            var queue = _queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();

            try
            {
                // sender
                var sender = new QueueChannelSender(
                    options =>
                    {
                        options.QueueName = queueName;
                        options.ConnectionString = "UseDevelopmentStorage=true;";
                    });

                // receiver
                var receiver = new QueueChannelReceiver(
                    options =>
                    {
                        options.QueueName = queueName;
                        options.ConnectionString = "UseDevelopmentStorage=true;";
                        options.SleepBetweenMessages = TimeSpan.FromMilliseconds(10);
                    });
                var task = receiver.StartAsync(OnMessage);

                // send message
                var response = await sender.SendAsync(
                                   new Message
                                   {
                                       ["request"] = "Hello world",
                                       ["queueName"] = queueName
                                   });

                // process result
                Assert.IsNotNull(response);
                Assert.AreEqual(Message.Empty, response);

                var delayTask = Task.Delay(30000);
                while (!delayTask.IsCompletedSuccessfully)
                {
                    if (Responses.ContainsKey(queueName))
                    {
                        break;
                    }
                }

                Assert.IsTrue(Responses.ContainsKey(queueName));
                response = Responses[queueName];
                Assert.AreEqual(":-)", response["response"]);

                // stop server
                receiver.Stop();
            }
            finally
            {
                // delete queue
                await queue.DeleteIfExistsAsync();
            }
        }

        private static Task<Message> OnMessage(Message msg, IChannelReceiver receiver)
        {
            var message = new Message
            {
                ["response"] = ":-)"
            };

            Responses[(receiver as QueueChannelReceiver).QueueName] = message;

            return Task.FromResult(message);
        }

        private static string GetQueueName()
        {
            var number = Generator.Next(1000, 2000);
            return $"unittest-{number}-items";
        }
    }
}
