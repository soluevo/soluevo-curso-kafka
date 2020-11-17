using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Producer.Domain;

namespace Producer.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<IPaymentService> _logger;
        private readonly IProducer<string, Payment> _producerPayment;
        private readonly IProducer<string, RollbackPayment> _producerRollback;

        public PaymentService(ILogger<IPaymentService> logger, IProducer<string, Payment> producerPayment, 
            IProducer<string, RollbackPayment> producerRollback)
        {
            _logger = logger;
            _producerPayment = producerPayment;
            _producerRollback = producerRollback;
        }

        public Task<Payment> Pay(Payment payment)
        {
            throw new System.NotImplementedException();
        }

        public Task<RollbackPayment> Rollback(RollbackPayment rollback)
        {
            throw new System.NotImplementedException();
        }
    }
}