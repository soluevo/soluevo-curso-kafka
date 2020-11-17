using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConsumer<Ignore, string> consumer, IConfiguration configuration)
        {
            _logger = logger;
            _consumer = consumer;
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () => await ConsumeAnima(stoppingToken), stoppingToken);
            return Task.CompletedTask;
        }

        private async Task ConsumeAnima(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_configuration["Kafka:TopicAnima"]);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker Consumer running at: {time}", DateTimeOffset.Now);
                    try
                    {
                        var result = _consumer.Consume(stoppingToken);
                        _logger.LogInformation("Result {}", result.Message.Value);
                        await Task.Delay(1000, stoppingToken);
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError(e, "Error consuming");
                    }
                }
            }
            finally
            {
                _consumer.Close();    
            }
        }
    }
}