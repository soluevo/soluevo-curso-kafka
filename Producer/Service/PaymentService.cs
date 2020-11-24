using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Producer.Cross;
using Producer.Domain;

namespace Producer.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<IPaymentService> _logger;
        private readonly IProducer<string, Payment> _producerPayment;
        private readonly IProducer<string, RollbackPayment> _producerRollback;
        private readonly IConfiguration _configuration;

        public PaymentService(ILogger<IPaymentService> logger, IProducer<string, Payment> producerPayment, 
            IProducer<string, RollbackPayment> producerRollback, IConfiguration configuration)
        {
            _logger = logger;
            _producerPayment = producerPayment;
            _producerRollback = producerRollback;
            _configuration = configuration;
        }

        public Task<Notification<Payment>> Pay(Payment payment)
        {
            _logger.LogInformation("Receiving payment ID {}, Value {}", payment.Id, payment.Value);
            try
            {
                _producerPayment.Produce(_configuration["Kafka:TopicPayment"], new Message<string, Payment>()
                {
                    Key = payment.Id,
                    Value =  payment
                }, report =>
                {
                    if (report.Error.IsError)
                    {
                        _logger.LogError(report.Error.Reason);
                    }
                    else
                    {
                        _logger.LogInformation("Delivered with success");
                    }
                });

                return Task.FromResult(new Notification<Payment>
                {
                    Data = payment
                });
            }
            catch (ProduceException<string,Payment> e)
            {
                _logger.LogError(e, "Can't produce to topic {}", _configuration["Kafka:TopicPayment"]);
                return Task.FromResult(new Notification<Payment>
                {
                    Error = $"Can't produce to topic {_configuration["Kafka:TopicPayment"]}. {e.Message}"
                });
            }
        }

        public async Task<Notification<RollbackPayment>> Rollback(RollbackPayment rollback)
        {
            _logger.LogInformation("Receiving rollback to payment ID {}, Reason {}", rollback.IdPayment, rollback.Reason);
            try
            {
                await _producerRollback.ProduceAsync(_configuration["Kafka:TopicRollback"], new Message<string, RollbackPayment>()
                {
                    Key = rollback.IdPayment,
                    Value =  rollback
                });

                return new Notification<RollbackPayment>
                {
                    Data = rollback
                };
            }
            catch (ProduceException<string,RollbackPayment> e)
            {
                _logger.LogError(e, "Can't produce to topic {}", _configuration["Kafka:TopicRollback"]);
                return new Notification<RollbackPayment>
                {
                    Error = $"Can't produce to topic {_configuration["Kafka:TopicRollback"]}. {e.Message}" 
                };
            }
        }
    }
}