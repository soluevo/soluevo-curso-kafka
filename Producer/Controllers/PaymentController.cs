using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Producer.Domain;
using Producer.Service;

namespace Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {

        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("pay")]
        public async Task<ActionResult> Pay([FromBody] Payment payment)
        {
            var notification = await _paymentService.Pay(payment);
            return notification.Error == null ? Accepted(payment) : Problem(notification.Error);
        }
        
        [HttpPost("rollback")]
        public async Task<ActionResult> Rollback([FromBody] RollbackPayment rollback)
        {
            var notification = await _paymentService.Rollback(rollback);
            return notification.Error == null ? Accepted(rollback) : Problem(notification.Error);
        }
        
    }
}