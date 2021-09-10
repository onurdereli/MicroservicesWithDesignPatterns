﻿using System;
using System.Collections.Generic;
using MassTransit;

namespace Shared.Events.Abstract
{
    public interface IPaymentFailedEvent: CorrelatedBy<Guid>
    {
        public string Reason { get; set; }

        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
