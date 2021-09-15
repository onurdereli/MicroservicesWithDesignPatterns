using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EventSourcing.Shared.Event;
using EventStore.ClientAPI;

namespace EventSourcing.API.EventStores
{
    public abstract class AbstractStream
    {
        protected readonly List<IEvent> Events;

        private string _streamName { get; set; }

        private readonly IEventStoreConnection _eventStoreConnection;

        protected AbstractStream(IEventStoreConnection eventStoreConnection, string streamName)
        {
            Events = new List<IEvent>();
            _eventStoreConnection = eventStoreConnection;
            _streamName = streamName;
        }

        public async Task SaveAsync()
        {
            var newEvents = Events.Select(x =>
                new EventData(
                    Guid.NewGuid(),
                    x.GetType().Name,
                    true,
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(x, inputType: x.GetType())),
                    Encoding.UTF8.GetBytes(x.GetType().FullName))
                    )
                .ToList();

            await _eventStoreConnection.AppendToStreamAsync(_streamName, ExpectedVersion.Any, newEvents);

            Events.Clear();
        }
    }
}
