using System;
using System.Collections.Generic;
using MassTransit;

namespace Shared.Events.Abstract
{
    public interface IOrderCreatedEvent: CorrelatedBy<Guid>
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
