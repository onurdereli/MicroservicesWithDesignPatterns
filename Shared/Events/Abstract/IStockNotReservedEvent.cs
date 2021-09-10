using System;
using MassTransit;

namespace Shared.Events.Abstract
{
    public interface IStockNotReservedEvent: CorrelatedBy<Guid>
    {
        public string Reason { get; set; }
    }
}
