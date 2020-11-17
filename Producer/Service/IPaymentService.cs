using System.Threading.Tasks;
using Producer.Cross;
using Producer.Domain;

namespace Producer.Service
{
    public interface IPaymentService
    {
        public Task<Notification<Payment>> Pay(Payment payment);
        public Task<Notification<RollbackPayment>> Rollback(RollbackPayment rollback);
    }
}