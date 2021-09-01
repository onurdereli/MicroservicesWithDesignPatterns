using Shared.Events.Abstract;

namespace Shared.Events.Concrete
{
    public class PaymentCompletedEvent : IPaymentCompletedEvent
    {
        public int OrderId { get; set; }

        public string BuyerId { get; set; }
    }
}
