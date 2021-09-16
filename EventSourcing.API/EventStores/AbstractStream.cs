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

        private string StreamName { get; set; }

        private readonly IEventStoreConnection _eventStoreConnection;

        protected AbstractStream(IEventStoreConnection eventStoreConnection, string streamName)
        {
            Events = new List<IEvent>();
            _eventStoreConnection = eventStoreConnection;
            StreamName = streamName;
        }

        public async Task SaveAsync()
        {
            var newEvents = Events.Select(eventInfo =>
                new EventData(
                    Guid.NewGuid(),
                    eventInfo.GetType().Name,
                    true,
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventInfo, inputType: eventInfo.GetType())),
                    Encoding.UTF8.GetBytes(eventInfo.GetType().FullName))
                    )
                .ToList();
            
            await _eventStoreConnection.AppendToStreamAsync(StreamName, ExpectedVersion.Any, newEvents);
            
            Events.Clear();
        }
    }
}
