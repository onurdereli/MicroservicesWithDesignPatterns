using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcing.Shared.Event;

namespace EventSourcing.Shared
{
    public class ProductPriceChangedEvent : IEvent
    {
        public Guid Id { get; set; }

        public decimal ChangedPrice { get; set; }
    }
}
