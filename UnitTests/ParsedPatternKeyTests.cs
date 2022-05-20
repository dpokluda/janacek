using Janacek;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class ParsedPatternKeyTests
    {
        [TestMethod]
        public void OneElementPattern()
        {
            var key = new ParsedPatternKey(new ParsedPattern("one:1"));
            Assert.IsNotNull(key.Elements);
            Assert.AreEqual(1, key.Elements.Count);
            Assert.AreEqual("one", key.Elements[0]);
        }

        [TestMethod]
        public void TwoElementsPattern()
        {
            var key = new ParsedPatternKey(new ParsedPattern("  one: 1, TWO:2  "));
            Assert.IsNotNull(key.Elements);
            Assert.AreEqual(2, key.Elements.Count);
            Assert.AreEqual("one", key.Elements[0]);
            Assert.AreEqual("two", key.Elements[1]);
        }

        [TestMethod]
        public void Comparer()
        {
            var comparer = new ParsedPatternKeyComparer();

            // more specific is better (smaller)
            Assert.IsTrue(comparer.Compare(
                              new ParsedPatternKey(new ParsedPattern("one:1,two:2")),
                              new ParsedPatternKey(new ParsedPattern("one:1"))) < 0);

            // if same number of elements, then alphabetic order
            Assert.IsTrue(
                comparer.Compare(
                    new ParsedPatternKey(new ParsedPattern("one:1,a:2")),
                    new ParsedPatternKey(new ParsedPattern("one:1,b:2"))) < 0);

            // same if value is the only difference (since value is not part of the key)
            Assert.IsTrue(
                comparer.Compare(
                    new ParsedPatternKey(new ParsedPattern("one:1")),
                    new ParsedPatternKey(new ParsedPattern("one:2"))) == 0);
        }
    }
}
