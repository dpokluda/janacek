using System;
using System.Threading.Tasks;
using Janacek;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class MeshActorTest
    {
        private static readonly Random Generator = new Random((int)DateTime.UtcNow.Ticks);

        [TestMethod]
        public async Task SingleMachine()
        {
            var address1 = GetServiceAddress();
            var actor1 = new MeshActor()
                        .Add("cmd:sum", OnSum)
                        .Listen(
                            new HttpChannelReceiver(
                                options =>
                                {
                                    options.Address = address1;
                                }))
                        .Client(
                            new HttpChannelSender(
                                options =>
                                {
                                    options.Address = address1;
                                }))
                        .TreatAs<MeshActor>()
                        .Build(options =>
                        {
                            options.LocalPort = GetServiceAddress(); 
                            
                        })
                        .Run();
            Assert.IsNotNull(actor1);
            
            

            var response = await actor1.ActAsync(new Message
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
        public async Task TwoMachines()
        {
            // machine one - is empty
            var meshAddress1 = GetServiceAddress();
            var actor1 = new MeshActor()
                        .TreatAs<MeshActor>()
                        .Build(options =>
                        {
                            options.LocalPort = meshAddress1; 
                        })
                        .Run();
            Assert.IsNotNull(actor1);
            
            // machine two - listens for cmd:sum messages
            var address2 = GetServiceAddress();
            var actor2 = new MeshActor()
                         .Add("cmd:sum", OnSum)
                         .Listen(
                             new HttpChannelReceiver(
                                 options =>
                                 {
                                     options.Address = address2;
                                 }))
                         .Client(
                             new HttpChannelSender(
                                 options =>
                                 {
                                     options.Address = address2;
                                 }))
                         .TreatAs<MeshActor>()
                         .Build(options =>
                         {
                             options.LocalPort = GetServiceAddress();
                             options.KnownServers = new[] { meshAddress1 };
                         })
                         .Run();
            Assert.IsNotNull(actor1);            

            var response = await actor1.ActAsync(new Message
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