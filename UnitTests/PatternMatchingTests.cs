//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

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
        public void NumberMatch()
        {
            var pm = new PatternMatching<string>();
            var result = pm.Match(
                new Message
                {
                    { "role", "match" },
                    { "cmd", 2 }
                });

            Assert.IsNull(result);

            pm.Add("role:match, cmd:2", "true");

            result = pm.Match(
                new Message
                    {
                        { "role", "match" },
                        { "cmd", 2 }
                    });

            Assert.IsNotNull(result);
            Assert.AreEqual("true", result);
        }

        [TestMethod]
        public void AnyValueMatch()
        {
            var pm = new PatternMatching<string>();
            var result = pm.Match(
                new Message
                {
                    { "role", "match" },
                    { "cmd", "sum" }
                });

            Assert.IsNull(result);

            pm.Add("role:match, cmd:*", "true");

            result = pm.Match(
                new Message
                    {
                        { "role", "match" },
                        { "cmd", 2 }
                    });

            Assert.IsNotNull(result);
            Assert.AreEqual("true", result);

            result = pm.Match(
                new Message
                    {
                        { "role", "match" },
                        { "cmd", "sum" }
                    });

            Assert.IsNotNull(result);
            Assert.AreEqual("true", result);


            result = pm.Match(
                new Message
                    {
                        { "role", "match" },
                        { "cmd", null }
                    });

            Assert.IsNotNull(result);
            Assert.AreEqual("true", result);
        }

        [TestMethod]
        public void SimpleFind()
        {
            var pm = new PatternMatching<string>();
            var result = pm.Find("role:match,cmd:sum");
            Assert.IsNull(result);

            pm.Add("role:match, cmd:sum", "true");

            result = pm.Find("role:match,cmd:sum");

            Assert.IsNotNull(result);
            Assert.AreEqual("true", result.Value);
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
