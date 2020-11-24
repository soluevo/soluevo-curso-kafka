using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Consumer.Domain;
using Consumer.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConsumer<string, Payment> _consumerPayment;
        private readonly IConfiguration _configuration;
        private readonly IProcessPaymentService _processPaymentService;

        public Worker(ILogger<Worker> logger, IConsumer<string, Payment> consumerPayment, IConfiguration configuration, IProcessPaymentService processPaymentService)
        {
            _logger = logger;
            _consumerPayment = consumerPayment;
            _configuration = configuration;
            _processPaymentService = processPaymentService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () => await ConsumePayment(stoppingToken), stoppingToken);
            return Task.CompletedTask;
        }

        private async Task ConsumePayment(CancellationToken stoppingToken)
        {
            _consumerPayment.Subscribe(_configuration["Kafka:TopicPayment"]);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = _consumerPayment.Consume(stoppingToken);
                        await _processPaymentService.ProcessPayment(result.Message.Value);
                        _consumerPayment.Commit(result);
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
                _consumerPayment.Close();    
            }
        }
    }
}