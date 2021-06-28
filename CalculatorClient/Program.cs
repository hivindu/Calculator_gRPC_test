using CalculatorService;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace CalculatorClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var calculatorClient = new Calculator.CalculatorClient(channel);

            await UneryCall(client);
            Console.WriteLine("Let's do some random calculations !");
            Console.WriteLine("\n");
            await ClientStreamCall(calculatorClient);
            Console.WriteLine("\n");
            await ServerStreamCall(calculatorClient);
        }

        private static async Task UneryCall(Greeter.GreeterClient greeterClient)
        {
            Console.Write("Enter Your Name:");
            string _name = Convert.ToString(Console.ReadLine());
            var response = await greeterClient.SayHelloAsync(new HelloRequest
            {
                Name = _name
            });

            Console.WriteLine(response.Message);
        }

        private static async Task ClientStreamCall(Calculator.CalculatorClient calculatorClient)
        {
            System.Random random = new System.Random();

            var call = calculatorClient.Addition();
            Console.WriteLine("Let's get Addition of Three random Numbers");

            for (int i = 1; i < 4; i++)
            {
                var _randomNumber = random.Next(1, 100);
                await call.RequestStream.WriteAsync(new RequestModel { Value = _randomNumber });
            }

            await call.RequestStream.CompleteAsync();

            var response = await call;
            Console.WriteLine("Total is " + response.Total);
        }

        private static async Task ServerStreamCall(Calculator.CalculatorClient calculatorClient)
        {
            System.Random random = new System.Random();

            Console.WriteLine("Let's get Multiplication Table for random Number");
            var _randomNumber = random.Next(10);
            Console.WriteLine("Random Number is " + _randomNumber);
            var call = calculatorClient.MultiplicationTable(new RequestModel { Value = _randomNumber });

            await foreach (var item in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine(item.MultipliedBy + " X " + item.MultipliedValue + " = " + item.Result);
            }
        }
    }
}
