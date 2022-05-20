using Janacek;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class PatternMatchingTests
    {
        [TestMethod]
        public void SimpleMatch()
        {
            var pm = new PatternMatching<string>();
            var result = pm.Match(
                new Message
                {
                    { "role", "match" },
                    { "cmd", "sum" }
                });

            Assert.IsNull(result);

            pm.Add("role:match, cmd:sum", "true");

            result = pm.Match(
                new Message
                    {
                        { "role", "match" },
                        { "cmd", "sum" }
                    });

            Assert.IsNotNull(result);
            Assert.AreEqual("true", result);
        }

        [TestMethod]
        public void AllValuesMatch()
        {
            var pm = new PatternMatching<string>();

            pm.Add("role:*", "true");

            var result = pm.Match(
                new Message
                    {
                        { "role", "match" }
                    });

            Assert.IsNotNull(result);
            Assert.AreEqual("true", result);

            result = pm.Match(
                new Message
                    {
                        { "cmd", "match" }
                    });

            Assert.IsNull(result);
        }

        [TestMethod]
        public void AllMatch()
        {
            var pm = new PatternMatching<string>();

            pm.Add("*", "true");

            var result = pm.Match(
                new Message
                    {
                        { "role", "match" }
                    });

            Assert.IsNotNull(result);
            Assert.AreEqual("true", result);

            result = pm.Match(
                new Message
                    {
                        { "cmd", "match" }
                    });

            Assert.IsNotNull(result);
            Assert.AreEqual("true", result);

            result = pm.Match(
                Message.Empty);

            Assert.IsNotNull(result);
            Assert.AreEqual("true", result);
        }

        [TestMethod]
        public void AllMatch2()
        {
            var pm = new PatternMatching<string>();

            pm.Add("*", "1");
            pm.Add("role:*", "2");

            var result = pm.Match(
                new Message
                    {
                        { "role", "match" }
                    });

            Assert.IsNotNull(result);
            Assert.AreEqual("2", result);

            result = pm.Match(
                new Message
                    {
                        { "cmd", "match" }
                    });

            Assert.IsNotNull(result);
            Assert.AreEqual("1", result);

        }

        [TestMethod]
        public void AllMatch3()
        {
            var pm = new PatternMatching<string>();

            pm.Add("*", "1");
            pm.Add("role:match", "3");
            // TODO: order
            pm.Add("role:*", "2");

            var result = pm.Match(
                new Message
                    {
                        { "role", "match" }
                    });

            Assert.IsNotNull(result);
            Assert.AreEqual("3", result);

            result = pm.Match(
                new Message
                    {
                        { "role", "match2" }
                    });

            Assert.IsNotNull(result);
            Assert.AreEqual("2", result);

            result = pm.Match(
                new Message
                {
                    { "cmd", "match" }
                });

            Assert.IsNotNull(result);
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void Sorting()
        {
            var pm = new PatternMatching<string>();
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
            var pm = new PatternMatching<string>();
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
            var pm = new PatternMatching<string>();
            pm.Add("a:1", "1");

            var result = pm.Match(
                new Message
                    {
                        { "a", "1" },
                    });
            Assert.AreEqual("1", result);

            pm.Add("a:1, b:2", "2");
            result = pm.Match(
                new Message
                    {
                        { "a", "1" },
                        { "b", "2" }
                    });

            Assert.AreEqual("2", result);
        }
    }
}
