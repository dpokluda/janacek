using Janacek;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class ParsedPatternTests
    {
        [TestMethod]
        public void OneElementPattern()
        {
            var pattern = new ParsedPattern("one:1");
            Assert.IsNotNull(pattern.Elements);
            Assert.AreEqual(1, pattern.Elements.Count);
            Assert.AreEqual("one", pattern.Elements[0].Key);
            Assert.AreEqual("1", pattern.Elements[0].Value);
        }

        [TestMethod]
        public void TwoElementsPattern()
        {
            var pattern = new ParsedPattern("  one: 1, TWO:2  ");
            Assert.IsNotNull(pattern.Elements);
            Assert.AreEqual(2, pattern.Elements.Count);
            Assert.AreEqual("one", pattern.Elements[0].Key);
            Assert.AreEqual("1", pattern.Elements[0].Value);
            Assert.AreEqual("two", pattern.Elements[1].Key);
            Assert.AreEqual("2", pattern.Elements[1].Value);
        }

        [TestMethod]
        public void IsMatchOneElementPattern()
        {
            var pattern = new ParsedPattern("one:1");
            Assert.IsTrue(
                pattern.IsMatch(
                    new Message
                    {
                        ["one"] = "1"
                    }));

            Assert.IsFalse(
                pattern.IsMatch(
                    new Message
                    {
                        ["one"] = "2"
                    }));
        }

        [TestMethod]
        public void IsMatchTwoElementsPattern()
        {
            var pattern = new ParsedPattern("  one: 1, TWO:2  ");
            Assert.IsTrue(
                pattern.IsMatch(
                    new Message
                    {
                        ["one"] = "1",
                        ["two"] = "2"
                    }));

            Assert.IsFalse(
                pattern.IsMatch(
                    new Message
                    {
                        ["one"] = "1"
                    }));
            Assert.IsFalse(
                pattern.IsMatch(
                    new Message
                    {
                        ["one"] = "2"
                    }));
        }

        [TestMethod]
        public void IsMatchCaseInsensitive()
        {
            var pattern = new ParsedPattern("one:TWO");
            Assert.IsTrue(
                pattern.IsMatch(
                    new Message
                    {
                        ["ONE"] = "two"
                    }));
        }
    }
}
