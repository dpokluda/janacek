using System;
using System.Threading.Tasks;
using Janacek;

namespace Telemetry
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("******************************************************************************");
            Console.WriteLine("*                                                                            *");
            Console.WriteLine("* Telemetry - listens to all \"role:telemetry\" events                       *");
            Console.WriteLine("*                                                                            *");
            Console.WriteLine("******************************************************************************");
            Console.WriteLine("Running...");
            Console.WriteLine();

            Console.WriteLine("Events:");

            // run main async method
            RunAsync()
                .GetAwaiter()
                .GetResult();
        }

        private static async Task RunAsync()
        {
            var main = new JanacekConsumer()
                       .Add("role:telemetry", OnTelemetryMessage)
                       .Listen(new QueueChannelReceiver(
                           options =>
                           {
                               options.QueueName = "telemetry-items";
                               options.ConnectionString = "UseDevelopmentStorage=true;";
                           }))
                       .Run();

            await main;
        }

        private static Task<Message> OnTelemetryMessage(Message msg)
        {
            Console.WriteLine($"    {msg.Serialize()}");
            return Task.FromResult(Message.Empty);
        }
    }
}
