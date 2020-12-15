using System.Threading.Tasks;
using Consumer.Cross;
using Consumer.Domain;
using Producer.Domain;

namespace Consumer.Service
{
    public interface IProcessPaymentService
    {
        public Task<Notification<Payment>> ProcessPayment(Payment payment);
        public Task<Notification<RollbackPayment>> ProcessRollback(RollbackPayment rollback);
    }
}