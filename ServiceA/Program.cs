using System;
using Janacek;

namespace ServiceA
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("******************************************************************************");
            Console.WriteLine("*                                                                            *");
            Console.WriteLine("* MathService - microservice to calculate SUM and PRODUCT of two numbers     *");
            Console.WriteLine("*                                                                            *");
            Console.WriteLine("******************************************************************************");
            Console.WriteLine("Running...");
            Console.WriteLine();
            Console.WriteLine("Received messages:");
            var task = new JanacekConsumer()
                       .Add("role:math,cmd:sum", MathService.OnSum)
                       .Add("role:math,cmd:product", MathService.OnProduct)
                       .Listen(new HttpChannelReceiver(
                           options =>
                           {
                               options.ServiceAddress = "http://localhost:8101/";
                           }))
                       .Run();

            var direct = new MessageConsumer()
                         .Add("role:math,cmd:product", MathService.OnProduct)
                         .Listen(new HttpChannelReceiver(
                             options =>
                             {
                                 options.ServiceAddress = "http://localhost:8102/";
                             }))
                         .Run();

            task.GetAwaiter().GetResult();
            direct.GetAwaiter().GetResult();

            Console.WriteLine("Finished.");
        }
    }
}
