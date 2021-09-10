using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Events.Abstract;

namespace Shared.Events.Concrete
{
    public class StockReservedRequestPayment : IStockReservedRequestPayment
    {
        public StockReservedRequestPayment(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }

        public PaymentMessage Payment { get; set; }

        public List<OrderItemMessage> OrderItems { get; set; }

        public string BuyerId { get; set; }
    }
}
