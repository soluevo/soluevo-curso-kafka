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
        private readonly IConsumer<string, RollbackPayment> _consumerRollbackPayment;
        private readonly IConfiguration _configuration;
        private readonly IProcessPaymentService _processPaymentService;

        public Worker(ILogger<Worker> logger, IConsumer<string, Payment> consumerPayment, IConfiguration configuration, 
            IProcessPaymentService processPaymentService, IConsumer<string, RollbackPayment> consumerRollbackPayment)
        {
            _logger = logger;
            _consumerPayment = consumerPayment;
            _configuration = configuration;
            _processPaymentService = processPaymentService;
            _consumerRollbackPayment = consumerRollbackPayment;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () => await ConsumePayment(stoppingToken), stoppingToken);
            //Task.Run(async () => await ConsumeRollback(stoppingToken), stoppingToken);
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
                        //_consumerPayment.Commit(result);
                        _consumerPayment.StoreOffset(result);
                        await Task.Delay(1000, stoppingToken);
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError(e, "Error consuming");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _consumerPayment.Close();    
            }
        }
        
        private async Task ConsumeRollback(CancellationToken stoppingToken)
        {
            _consumerRollbackPayment.Subscribe(_configuration["Kafka:TopicRollbackPayment"]);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = _consumerRollbackPayment.Consume(stoppingToken);
                        await _processPaymentService.ProcessRollback(result.Message.Value);
                        _consumerRollbackPayment.Commit(result);
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
                _consumerRollbackPayment.Close();    
            }
        }
    }
}