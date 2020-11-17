using System.Threading.Tasks;
using Producer.Domain;

namespace Producer.Service
{
    public interface IPaymentService
    {
        public Task<Payment> Pay(Payment payment);
        public Task<RollbackPayment> Rollback(RollbackPayment rollback);
    }
}