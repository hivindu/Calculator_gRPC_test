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
            int _total = 0;
            AdditionResponseModel response = new AdditionResponseModel();
            await foreach (var number in requestStream.ReadAllAsync())
            {
                _total += number.Value;
            }

            response.Total = _total;
            _logger.LogInformation($"Total is { _total }");

            return response;
        }
        public override async Task MultiplicationTable(RequestModel request, IServerStreamWriter<MultiplicationResponseModel> responseStream, ServerCallContext context)
        {
            int _result = 0;
            for (int i = 1; i < 11; i++)
            {
                _result = i * request.Value;

                await responseStream.WriteAsync( new MultiplicationResponseModel { 
                
                    MultipliedValue = i,
                    MultipliedBy = request.Value,
                    Result = _result
                });

               await Task.Delay(TimeSpan.FromSeconds(1));

            }
        }
    }
}
