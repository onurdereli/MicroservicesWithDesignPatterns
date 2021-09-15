using System;
using EventSourcing.API.Dtos;
using EventSourcing.Shared;
using EventStore.ClientAPI;

namespace EventSourcing.API.EventStores
{
    public class ProductStream : AbstractStream
    {
        public static string StreamName => "ProductStream";

        public ProductStream(IEventStoreConnection eventStoreConnection) : base(eventStoreConnection, StreamName)
        {

        }

        public void Created(CreateProductDto createProductDto)
        {
            Events.Add(new ProductCreatedEvent
            {
                Id = Guid.NewGuid(),
                Name = createProductDto.Name,
                Price = createProductDto.Price,
                Stock = createProductDto.Stock,
                UserId = createProductDto.UserId
            });
        }

        public void NameChanged(ChangeProductNameDto changeProductNameDto)
        {
            Events.Add(new ProductNameChangedEvent
            {
                ChangedName = changeProductNameDto.Name,
                Id = changeProductNameDto.Id
            });
        }

        public void PriceChanged(ChangeProductPriceDto changeProductPriceDto)
        {
            Events.Add(new ProductPriceChangedEvent
            {
                ChangedPrice = changeProductPriceDto.Price,
                Id = changeProductPriceDto.Id
            });
        }

        public void Deleted(Guid id)
        {
            Events.Add(new ProductDeletedEvent
            {
                Id = id
            });
        }
    }
}
