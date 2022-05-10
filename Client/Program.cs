using System;
using System.Collections.Generic;
using JanacekClient;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Press [Enter] to start the app...");
            Console.ReadLine();
            
            Console.WriteLine("Calling Math.Sum microservice to sum numbers 2.2 and 3.3: ");

            var janacek = new JanacekProducer();
            var result = janacek.Act(new Message(new Dictionary<string, object>
            {
                { "role", "math" },
                { "cmd", "sum" },
                { "left", 2.2 },
                { "right", 3.3 }
            })).Result;

            if (result != null && result.ContainsKey("sum"))
            {
                var sum = result["sum"];
                Console.WriteLine($"Result: {sum}");
            }
            else
            {
                Console.WriteLine("Error occured!");
            }

            Console.Write("Press [Enter] to quit the app...");
            Console.ReadLine();
            
        }
    }
}
