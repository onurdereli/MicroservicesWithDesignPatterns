using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcing.Shared.Event;

namespace EventSourcing.Shared
{
    class ProductCreatedEvent: IEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int UserId { get; set; }
    }
}
