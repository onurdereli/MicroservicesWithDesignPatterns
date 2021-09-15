using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcing.Shared.Event;

namespace EventSourcing.Shared
{
    public class ProductNameChangedEvent: IEvent
    {
        public Guid Id { get; set; }

        public string ChangedName { get; set; }
    }
}
