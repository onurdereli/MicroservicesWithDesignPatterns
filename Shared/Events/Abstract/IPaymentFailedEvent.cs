using System.Collections.Generic;

namespace Shared.Events.Abstract
{
    public interface IPaymentFailedEvent
    {
        public int OrderId { get; set; }

        public string BuyerId { get; set; }

        public string Message { get; set; }

        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
