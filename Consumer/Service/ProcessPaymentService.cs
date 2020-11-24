using System.Threading.Tasks;
using Consumer.Cross;
using Consumer.Domain;
using Microsoft.Extensions.Logging;

namespace Consumer.Service
{
    public class ProcessPaymentService : IProcessPaymentService
    {
        private readonly ILogger<IProcessPaymentService> _logger;

        public ProcessPaymentService(ILogger<IProcessPaymentService> logger)
        {
            _logger = logger;
        }

        public Task<Notification<Payment>> ProcessPayment(Payment payment)
        {
            _logger.LogInformation("Processing payment ID {}, with value {}", payment.Id, payment.Value);
            return Task.FromResult(new Notification<Payment>
            {
                Data = payment
            });
        }

        public Task<Notification<RollbackPayment>> ProcessRollback(RollbackPayment rollback)
        {
            _logger.LogInformation("Processing rollback payment ID {}, Reason {}", rollback.IdPayment, rollback.Reason);
            return Task.FromResult(new Notification<RollbackPayment>
            {
                Data = rollback
            });
        }
    }
}