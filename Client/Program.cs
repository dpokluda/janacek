using System;
using Janacek;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("******************************************************************************");
            Console.WriteLine("*                                                                            *");
            Console.WriteLine("* Math client - sends simple messages to Math microservice                   *");
            Console.WriteLine("*                                                                            *");
            Console.WriteLine("******************************************************************************");
            Console.WriteLine();

            Console.Write("Press [Enter] to start the app...");
            Console.ReadLine();
            
            string answer = string.Empty;
            do
            {
                Console.WriteLine("Sending request using Janacek service...");

                var producer = new JanacekProducer();
                var response = producer.ActAsync(
                                           new Message
                                           {
                                               ["role"] = "math",
                                               ["cmd"] = "sum",
                                               ["left"] = 10,
                                               ["right"] = 20,
                                           })
                                       .GetAwaiter()
                                       .GetResult();
                Console.WriteLine($"    SUM response: {response.Serialize()}");

                response = producer.ActAsync(
                                       new Message
                                       {
                                           ["role"] = "math",
                                           ["cmd"] = "product",
                                           ["left"] = 11,
                                           ["right"] = 22,
                                       })
                                   .GetAwaiter()
                                   .GetResult();
                Console.WriteLine($"    PRODUCT response: {response.Serialize()}");

                Console.WriteLine("Sending request directly...");

                response = new MessageConsumer()
                           .Client(
                               new HttpChannelSender(
                                   options => { options.ServiceAddress = "http://localhost:8102/"; }))
                           .ActAsync(
                               new Message
                               {
                                   ["role"] = "math",
                                   ["cmd"] = "product",
                                   ["left"] = 1.1,
                                   ["right"] = 2.2,
                               })
                           .GetAwaiter()
                           .GetResult();
                Console.WriteLine($"    PRODUCT response: {response.Serialize()}");
                Console.WriteLine();

                Console.Write("Enter 'yes' to run the app again or anything else to quit the app (default: yes)...");
                answer = Console.ReadLine();
                Console.WriteLine();
            }
            while (answer.Equals("yes", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(answer));
        }
    }
}
