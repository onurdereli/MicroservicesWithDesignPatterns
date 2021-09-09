using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Events.Abstract;

namespace Shared.Events.Concrete
{
    public class OrderCreatedRequestEvent: IOrderCreatedRequestEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
        public PaymentMessage Payment { get; set; }

        public OrderCreatedRequestEvent()
        {
            OrderItems = new List<OrderItemMessage>();
        }
    }
}
