using System.Collections.Generic;
using Shared.Events.Abstract;

namespace Shared.Events.Concrete
{
    public class PaymentFailedEvent : IPaymentFailedEvent
    {
        public int OrderId { get; set; }

        public string BuyerId { get; set; }

        public string Message { get; set; }

        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
