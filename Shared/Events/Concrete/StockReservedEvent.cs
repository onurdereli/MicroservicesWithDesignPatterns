using System.Collections.Generic;
using Shared.Events.Abstract;

namespace Shared.Events.Concrete
{
    public class StockReservedEvent: IStockReservedEvent
    {
        public int OrderId { get; set; }

        public string BuyerId { get; set; }

        public PaymentMessage Payment { get; set; }

        public List<OrderItemMessage> OrderItems { get; set; }

        public StockReservedEvent()
        {
            OrderItems = new List<OrderItemMessage>();
        }
    }
}
