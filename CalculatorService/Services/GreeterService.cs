using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CalculatorService
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            if (request is null)
            {
                _logger.LogError("Request is null");
            }

            return Task.FromResult(new HelloReply
            {
                Message = "Welcome " + request.Name
            });
        }
    }
}
