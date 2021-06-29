using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CalculatorService.Services
{
    public class CalculationService : Calculator.CalculatorBase
    {
        private readonly ILogger<CalculationService> _logger;

        public CalculationService(ILogger<CalculationService> logger)
        {
            _logger = logger;
        }

        public override async Task<AdditionResponseModel> Addition(IAsyncStreamReader<RequestModel> requestStream, ServerCallContext context)
        {
            // CHeck for async and await
            int total = 0;
            AdditionResponseModel response = new();
            await foreach (var number in requestStream.ReadAllAsync())
            {
                total += number.Value;
            }

            response.Total = total;
            _logger.LogInformation($"Total is { total }");

            return response;
        }
        public override async Task MultiplicationTable(RequestModel request, IServerStreamWriter<MultiplicationResponseModel> responseStream, ServerCallContext context)
        {
            for (int i = 1; i < 11; i++)
            {
                int result = i * request.Value;

                await responseStream.WriteAsync(new MultiplicationResponseModel
                {
                    MultipliedValue = i,
                    MultipliedBy = request.Value,
                    Result = result
                }).ConfigureAwait(false);

                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            }
        }
    }
}
