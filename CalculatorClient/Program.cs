using CalculatorService;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace CalculatorClient
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var calculatorClient = new Calculator.CalculatorClient(channel);

            await UneryCall(client).ConfigureAwait(false);
            Console.WriteLine("Let's do some random calculations !");
            Console.WriteLine("\n");
            await ClientStreamCall(calculatorClient).ConfigureAwait(false);
            Console.WriteLine("\n");
            await ServerStreamCall(calculatorClient).ConfigureAwait(false);
        }

        private static async Task UneryCall(Greeter.GreeterClient greeterClient)
        {
            Console.Write("Enter Your Name:");
            string name = Convert.ToString(Console.ReadLine());
            var response = await greeterClient.SayHelloAsync(new HelloRequest
            {
                Name = name
            });

            Console.WriteLine(response.Message);
        }

        private static async Task ClientStreamCall(Calculator.CalculatorClient calculatorClient)
        {
            Random random = new();

            var call = calculatorClient.Addition();
            Console.WriteLine("Let's get Addition of Three random Numbers");

            for (int i = 1; i < 4; i++)
            {
                var randomNumber = random.Next(1, 100);
                await call.RequestStream.WriteAsync(new RequestModel { Value = randomNumber }).ConfigureAwait(false);
            }

            await call.RequestStream.CompleteAsync().ConfigureAwait(false);

            var response = await call;
            Console.WriteLine("Total is " + response.Total);
        }

        private static async Task ServerStreamCall(Calculator.CalculatorClient calculatorClient)
        {
           Random random = new();

            Console.WriteLine("Let's get Multiplication Table for random Number");
            var randomNumber = random.Next(10);
            Console.WriteLine("Random Number is " + randomNumber);
            var call = calculatorClient.MultiplicationTable(new RequestModel { Value = randomNumber });

            await foreach (var item in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine(item.MultipliedBy + " X " + item.MultipliedValue + " = " + item.Result);
            }
        }
    }
}
