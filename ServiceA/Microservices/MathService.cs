using System;
using System.Collections.Generic;
using JanacekClient;

namespace ServiceA.Microservices
{
    public class MathService
    {
        public static Message OnSum(Message msg)
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

        public static Message OnProduct(Message msg)
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
    }
}
