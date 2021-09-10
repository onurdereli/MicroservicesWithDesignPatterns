using System;
using System.Collections.Generic;
using Shared.Events.Abstract;

namespace Shared.Events.Concrete
{
    public class OrderCreatedEvent : IOrderCreatedEvent
    {
        public List<OrderItemMessage> OrderItems { get; set; }

        public OrderCreatedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }
    }
}
