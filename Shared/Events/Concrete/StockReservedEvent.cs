using System;
using System.Collections.Generic;
using Shared.Events.Abstract;

namespace Shared.Events.Concrete
{
    public class StockReservedEvent: IStockReservedEvent
    {
        public List<OrderItemMessage> OrderItems { get; set; }

        public StockReservedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }
    }
}
