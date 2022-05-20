using System;
using System.Threading.Tasks;
using Janacek;

namespace ServiceA
{
    public class MathService
    {
        public static Task<Message> OnSum(Message msg)
        {
            Console.WriteLine($"    {msg.Serialize()}");
            var result = new Message
            {
                ["sum"] = Convert.ToInt32(msg["left"]) + Convert.ToInt32(msg["right"])
            };
            return Task.FromResult(result);
        }

        public static Task<Message> OnProduct(Message msg)
        {
            Console.WriteLine($"    {msg.Serialize()}");
            var result = new Message
            {
                ["product"] = Convert.ToInt32(msg["left"]) * Convert.ToInt32(msg["right"])
            };
            return Task.FromResult(result);
        }
    }
}
