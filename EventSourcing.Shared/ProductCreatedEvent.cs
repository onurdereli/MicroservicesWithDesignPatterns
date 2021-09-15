using System;
using EventSourcing.Shared.Event;

namespace EventSourcing.Shared
{
    public class ProductCreatedEvent: IEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int UserId { get; set; }
    }
}
