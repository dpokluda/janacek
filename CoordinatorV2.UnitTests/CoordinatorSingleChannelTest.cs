//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Janacek;
using Janacek.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace UnitTests
{
    [TestClass]
    public class CoordinatorSingleChannelTest
    {
        private const string ConnectionString = "UseDevelopmentStorage=true;";

        private static readonly Random Generator = new Random((int)DateTime.UtcNow.Ticks);

        private readonly CloudQueueClient _queueClient;
        private CloudQueue _coordinatorQueue;
        private CloudQueue _serviceQueue;

        private IActor _janacek;

        private bool _requestReceived;

        public CoordinatorSingleChannelTest()
        {
            var storageAccount = CloudStorageAccount.Parse(ConnectionString);
            _queueClient = storageAccount.CreateCloudQueueClient();
        }

        [TestInitialize]
        public async Task Setup()
        {
            // queue setup
            _coordinatorQueue = _queueClient.GetQueueReference(GetQueueName()+"-coordinator");
            await _coordinatorQueue.CreateIfNotExistsAsync();
            _serviceQueue = _queueClient.GetQueueReference(GetQueueName());
            await _serviceQueue.CreateIfNotExistsAsync();

            // start simulated Coordinator v2
            _janacek = new CoordinatorService(
                              options =>
                              {
                                  options.ConnectionString = ConnectionString;
                                  options.QueueName = _coordinatorQueue.Name;
                                  options.SleepBetweenMessages = TimeSpan.FromMilliseconds(10);
                              })
                          .Build()
                          .Run();
        }

        [TestCleanup]
        public async Task TearDown()
        {
            // delete queue
            await _coordinatorQueue.DeleteIfExistsAsync();
            await _serviceQueue.DeleteIfExistsAsync();

            // stop simulated Coordinator v2
            _janacek = null;
        }

        [TestMethod]
        public async Task Run()
        {
            new QueueChannelFactory();
            _requestReceived = false;

            var client = new CoordinatorClient(
                            options =>
                            {
                                options.ConnectionString = ConnectionString;
                                options.CoordinatorQueueName = _coordinatorQueue.Name;
                                options.SleepBetweenMessages = TimeSpan.FromMilliseconds(10);
                            })
                        .Build()
                        .Run();
            Assert.IsNotNull(client);

            var server = new CoordinatorClient(
                            options =>
                            {
                                options.ConnectionString = ConnectionString;
                                options.CoordinatorQueueName = _coordinatorQueue.Name;
                                options.ServiceQueueName = _serviceQueue.Name;
                                options.SleepBetweenMessages = TimeSpan.FromMilliseconds(10);
                            })
                        .Add("cmd:sum", OnSum)
                        .TreatAs<CoordinatorClient>()
                        .Build()
                        .TreatAs<CoordinatorClient>()
                        .Run();
            Assert.IsNotNull(server);

            Assert.IsFalse(_requestReceived);
            var response = await client.ActAsync(new Message
            {
                ["cmd"] = "sum",
                ["x"] = 11,
                ["y"] = 22
            });
            Assert.IsNotNull(response);

            var delayTask = Task.Delay(15000);
            while (!delayTask.IsCompletedSuccessfully)
            {
                if (_requestReceived)
                {
                    break;
                }
            }

            Assert.IsTrue(_requestReceived);
        }

        private Task<Message> OnSum(Message message)
        {
            _requestReceived = true;
            return Task.FromResult(new Message
            {
                ["sum"] = Convert.ToInt32(message["x"]) +
                          Convert.ToInt32(message["y"])
            });
        }

        private static string GetQueueName()
        {
            var number = Generator.Next(1000, 2000);
            return $"unittest-{number}-items";
        }
    }
}
