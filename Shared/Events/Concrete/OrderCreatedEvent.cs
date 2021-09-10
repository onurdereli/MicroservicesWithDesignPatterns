using System;
using System.Collections.Generic;
using Shared.Events.Abstract;

namespace Shared.Events.Concrete
{
    public class OrderCreatedEvent : IOrderCreatedEvent
    {
        public OrderCreatedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public List<OrderItemMessage> OrderItems { get; set; }
        
        public Guid CorrelationId { get; }
    }
}
