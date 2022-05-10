using System.Collections.Generic;
using JanacekClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class PatternMatchingTests
    {
        [TestMethod]
        public void SimpleMatch()
        {
            var pm = new TestablePatternMatching();
            var result = pm.Match(
                new Message(
                    new Dictionary<string, object>()
                    {
                        { "role", "match" },
                        { "cmd", "sum" }
                    }));

            Assert.IsNull(result);

            pm.Add("role:match, cmd:sum", "true");

            result = pm.Match(
                new Message(
                    new Dictionary<string, object>()
                    {
                        { "role", "match" },
                        { "cmd", "sum" }
                    }));

            Assert.IsNotNull(result);
            Assert.AreEqual("true", result);
        }

        [TestMethod]
        public void Sorting()
        {
            var pm = new TestablePatternMatching();
            pm.Add("a:1", "3");
            Assert.AreEqual(1, pm.Patterns.Count);
            Assert.AreEqual("3", pm.Patterns[0].Value);

            pm.Add("a:1, c:2", "2");
            Assert.AreEqual(2, pm.Patterns.Count);
            Assert.AreEqual("2", pm.Patterns[0].Value);
            Assert.AreEqual("3", pm.Patterns[1].Value);

            pm.Add("a:1, b:2", "1");
            Assert.AreEqual(3, pm.Patterns.Count);
            Assert.AreEqual("1", pm.Patterns[0].Value);
            Assert.AreEqual("2", pm.Patterns[1].Value);
            Assert.AreEqual("3", pm.Patterns[2].Value);

            pm.Add("d:4", "4");
            Assert.AreEqual(4, pm.Patterns.Count);
            Assert.AreEqual("1", pm.Patterns[0].Value);
            Assert.AreEqual("2", pm.Patterns[1].Value);
            Assert.AreEqual("3", pm.Patterns[2].Value);
            Assert.AreEqual("4", pm.Patterns[3].Value);
        }

        [TestMethod]
        public void Sorting2()
        {
            var pm = new TestablePatternMatching();
            pm.Add("a:1", "3");
            Assert.AreEqual(1, pm.Patterns.Count);
            Assert.AreEqual("3", pm.Patterns[0].Value);

            pm.Add("a:1, b:1", "1");
            Assert.AreEqual(2, pm.Patterns.Count);
            Assert.AreEqual("1", pm.Patterns[0].Value);
            Assert.AreEqual("3", pm.Patterns[1].Value);

            pm.Add("a:1, b:2", "2");
            Assert.AreEqual(3, pm.Patterns.Count);
            Assert.AreEqual("1", pm.Patterns[0].Value);
            Assert.AreEqual("2", pm.Patterns[1].Value);
            Assert.AreEqual("3", pm.Patterns[2].Value);
        }

        [TestMethod]
        public void OrderedMatch()
        {
            var pm = new TestablePatternMatching();
            pm.Add("a:1", "1");

            var result = pm.Match(
                new Message(
                    new Dictionary<string, object>()
                    {
                        { "a", "1" },
                    }));
            Assert.AreEqual("1", result);

            pm.Add("a:1, b:2", "2");
            result = pm.Match(
                new Message(
                    new Dictionary<string, object>()
                    {
                        { "a", "1" },
                        { "b", "2" }
                    }));

            Assert.AreEqual("2", result);
        }
    }
}
