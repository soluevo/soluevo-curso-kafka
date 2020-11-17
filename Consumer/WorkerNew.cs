using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer
{
    public class WorkerNew : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public WorkerNew(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () => await ConsumeAnima(stoppingToken),stoppingToken);
            return Task.CompletedTask;
        }

        private async Task ConsumeAnima(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:Servers"],
                GroupId = _configuration["Kafka:ConsumerGroupId"]  + "-new",
                ClientId = _configuration["Kafka:ClientId"] + "-" + Dns.GetHostName(),
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            
            var consumerNew = new ConsumerBuilder<Ignore, string>(config).Build();
            
            consumerNew.Subscribe(_configuration["Kafka:TopicAnima"]);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumerNew.Consume(stoppingToken);
                        _logger.LogInformation("Result-New {}", result.Message.Value);
                        await Task.Delay(1000, stoppingToken);
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError(e, "Error consuming-new");
                    }
                }
            }
            finally
            {
                consumerNew.Close();    
            }
        }
    }
}