using System.Collections.Generic;

namespace Shared.Events.Abstract
{
    public interface IOrderCreatedEvent
    {
        public int  OrderId { get; set; }

        public string BuyerId { get; set; }

        public PaymentMessage Payment { get; set; }

        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
