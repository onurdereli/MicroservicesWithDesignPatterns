using System;
using MassTransit;

namespace Shared.Events.Abstract
{
    public interface IPaymentCompletedEvent: CorrelatedBy<Guid>
    {

    }
}