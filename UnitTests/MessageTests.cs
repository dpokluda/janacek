using System.Collections.Generic;
using Janacek;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void ConstructFromDictionary()
        {
            var msg = new Message(
                new Dictionary<string, object>
                {
                    { "a", "1" },
                    { "b", "2" }
                });

            Assert.IsNotNull(msg);
            Assert.AreEqual(2, msg.Count);
            Assert.AreEqual("1", msg["a"]);
            Assert.AreEqual("2", msg["b"]);
        }

        [TestMethod]
        public void ConstructFromInitializer1()
        {
            var msg = new Message
                {
                    { "a",  "1" },
                    { "b", "2" }
                };

            Assert.IsNotNull(msg);
            Assert.AreEqual(2, msg.Count);
            Assert.AreEqual("1", msg["a"]);
            Assert.AreEqual("2", msg["b"]);
        }

        [TestMethod]
        public void ConstructFromInitializer2()
        {
            var msg = new Message
                {
                    ["a"] = "1",
                    ["b"] = "2" 
                };

            Assert.IsNotNull(msg);
            Assert.AreEqual(2, msg.Count);
            Assert.AreEqual("1", msg["a"]);
            Assert.AreEqual("2", msg["b"]);
        }

        [TestMethod]
        public void Enumeration()
        {
            var msg = new Message
                {
                    ["a"] = "1",
                    ["b"] = "2" 
                };

            Assert.IsNotNull(msg);
            int counter = 0;
            foreach ((string key, object value) in msg)
            {
                counter++;
                Assert.IsTrue(msg.ContainsKey(key));
                Assert.AreEqual(value, msg[key]);
            }
            Assert.AreEqual(msg.Count, counter);
        }

        [TestMethod]
        public void Serialize()
        {
            var msg = new Message
            {
                ["a"] = "1",
                ["b"] = "2"
            };

            var serialized = msg.Serialize();
            Assert.IsNotNull(serialized);
            Assert.AreEqual("{\"a\":\"1\",\"b\":\"2\"}", serialized);
        }

        [TestMethod]
        public void Deserialize()
        {
            var msg = Message.Deserialize("{\"a\":\"1\",\"b\":\"2\"}");

            Assert.AreEqual(2, msg.Count);
            Assert.AreEqual("1", msg["a"]);
            Assert.AreEqual("2", msg["b"]);
        }
    }
}
