using System;
using System.Collections.Generic;
using Shared.Events.Abstract;

namespace Shared.Events.Concrete
{
    public class StockReservedEvent: IStockReservedEvent
    {
        public StockReservedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public List<OrderItemMessage> OrderItems { get; set; }
        
        public Guid CorrelationId { get; }
    }
}
