using System;
using System.Collections.Generic;
using JanacekClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class SimpleTests
    {
        [TestMethod]
        public void Sum()
        {
            var janacek = new Janacek();
            janacek.Add("role:math, cmd:sum", OnSum);

            var result = janacek.Act(
                new Message(
                    new Dictionary<string, object>
                    {
                        { "role", "math" },
                        { "cmd", "sum" },
                        { "left", 1 },
                        { "right", 2 },
                    }));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey("sum"));
            Assert.AreEqual("3", result["sum"].ToString());

            result = janacek.Act(
                new Message(
                    new Dictionary<string, object>
                    {
                        { "role", "math" },
                        { "cmd", "sum" },
                        { "left", 1.1 },
                        { "right", 2.2 },
                    }));
            Assert.IsNotNull(result);
            Assert.AreEqual("3.3", result["sum"].ToString());
        }

        [TestMethod]
        public void SumAndProduct()
        {
            var janacek = new Janacek();
            janacek.Add("role:math, cmd:sum", OnSum);
            janacek.Add("role:math, cmd:product", OnProduct);

            var result = janacek.Act(
                new Message(
                    new Dictionary<string, object>
                    {
                        { "role", "math" },
                        { "cmd", "sum" },
                        { "left", 2 },
                        { "right", 3 },
                    }));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey("sum"));
            Assert.AreEqual("5", result["sum"].ToString());

            result = janacek.Act(
                new Message(
                    new Dictionary<string, object>
                    {
                        { "role", "math" },
                        { "cmd", "product" },
                        { "left", 2 },
                        { "right", 3 },
                    }));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey("product"));
            Assert.AreEqual("6", result["product"].ToString());
        }

        [TestMethod]
        public void SumAndProductWithIntegers()
        {
            var janacek = new Janacek();
            janacek.Add("role:math, cmd:sum", OnSum);
            janacek.Add("role:math, cmd:product", OnProduct);
            janacek.Add("role:math, cmd:sum, integer:true", OnIntegerSum);
            janacek.Add("role:math, cmd:product, integer:true", OnIntegerProduct);

            var result = janacek.Act(
                new Message(
                    new Dictionary<string, object>
                    {
                        { "role", "math" },
                        { "cmd", "sum" },
                        { "left", 2.2 },
                        { "right", 3.3 },
                    }));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey("sum"));
            Assert.AreEqual("5.5", result["sum"].ToString());

            result = janacek.Act(
                new Message(
                    new Dictionary<string, object>
                    {
                        { "role", "math" },
                        { "cmd", "sum" },
                        { "integer", "true" },
                        { "left", 2.2 },
                        { "right", 3.3 },
                    }));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey("sum"));
            Assert.AreEqual("5", result["sum"].ToString());

            result = janacek.Act(
                new Message(
                    new Dictionary<string, object>
                    {
                        { "role", "math" },
                        { "cmd", "product" },
                        { "left", 2.2 },
                        { "right", 3.3 },
                    }));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey("product"));
            Assert.AreEqual("7.26", result["product"].ToString());

            result = janacek.Act(
                new Message(
                    new Dictionary<string, object>
                    {
                        { "role", "math" },
                        { "cmd", "product" },
                        { "integer", "true" },
                        { "left", 2.2 },
                        { "right", 3.3 },
                    }));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey("product"));
            Assert.AreEqual("6", result["product"].ToString());
        }

        [TestMethod]
        public void Error()
        {
            var janacek = new Janacek();
            janacek.Add("role:math, cmd:sum", OnSum);

            try
            {
                var result = janacek.Act(
                    new Message(
                        new Dictionary<string, object>
                        {
                            { "role", "math" },
                            { "cmd", "sum" },
                            { "left", "a" },
                            { "right", "b" },
                        }));
                Assert.Fail("Exception expected");
            }
            catch (Exception exception)
            {
                Assert.IsTrue(exception is ArgumentOutOfRangeException);
            }

        }

        public Message OnSum(Message msg)
        {
            if (!float.TryParse(msg["left"].ToString(), out float left))
            {
                throw new ArgumentOutOfRangeException("left", "Left must be a number");
            }
            if (!float.TryParse(msg["right"].ToString(), out float right))
            {
                throw new ArgumentOutOfRangeException("right", "Right must be a number");
            }

            float sum = left + right;

            return new Message(new Dictionary<string, object>
            {
                { "sum", sum }
            });
        }

        public Message OnProduct(Message msg)
        {
            if (!float.TryParse(msg["left"].ToString(), out float left))
            {
                throw new ArgumentOutOfRangeException("left", "Left must be a number");
            }
            if (!float.TryParse(msg["right"].ToString(), out float right))
            {
                throw new ArgumentOutOfRangeException("right", "Right must be a number");
            }

            float product = left * right;

            return new Message(new Dictionary<string, object>
            {
                { "product", product }
            });
        }

        public Message OnIntegerSum(Message msg)
        {
            if (!float.TryParse(msg["left"].ToString(), out float left))
            {
                throw new ArgumentOutOfRangeException("left", "Left must be a number");
            }
            if (!float.TryParse(msg["right"].ToString(), out float right))
            {
                throw new ArgumentOutOfRangeException("right", "Right must be a number");
            }

            int sum = (int)Math.Floor(left) + (int)Math.Floor(right);

            return new Message(new Dictionary<string, object>
            {
                { "sum", sum }
            });
        }

        public Message OnIntegerProduct(Message msg)
        {
            if (!float.TryParse(msg["left"].ToString(), out float left))
            {
                throw new ArgumentOutOfRangeException("left", "Left must be a number");
            }
            if (!float.TryParse(msg["right"].ToString(), out float right))
            {
                throw new ArgumentOutOfRangeException("right", "Right must be a number");
            }

            int product = (int)Math.Floor(left) * (int)Math.Floor(right); ;

            return new Message(new Dictionary<string, object>
            {
                { "product", product }
            });
        }
    }
}
