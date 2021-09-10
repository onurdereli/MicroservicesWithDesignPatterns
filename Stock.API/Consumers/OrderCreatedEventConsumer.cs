using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Events.Abstract;
using Shared.Events.Concrete;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<IOrderCreatedEvent>
    {
        private readonly StockDbContext _stockDbContext;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(StockDbContext stockDbContext, ILogger<OrderCreatedEventConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _stockDbContext = stockDbContext;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IOrderCreatedEvent> context)
        {
            var stockResult = new List<bool>();

            foreach (var item in context.Message.OrderItems)
            {
                stockResult.Add(await _stockDbContext.Stocks.AnyAsync(x => x.ProductId == item.ProductId && x.Count > item.Count));
            }

            if (stockResult.All(x => x.Equals(true)))
            {
                // Ürün siparişi oluşturulduğunda stokta ürün varsa bütün ürünler alındığı kadarıyla stoktan düşülür
                foreach (var item in context.Message.OrderItems)
                {
                    var stock = await _stockDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                    if (stock != null)
                    {
                        stock.Count -= item.Count;
                    }

                    await _stockDbContext.SaveChangesAsync();
                }

                _logger.LogInformation($"Stock was reserved for Buyer Id :{context.Message.CorrelationId}");

                StockReservedEvent stockReservedEvent = new(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems
                };  
                //// Rezerve edilen stoğu ve güncel siparişleri publish edilir
                await _publishEndpoint.Publish(stockReservedEvent);

            }
            else
            {
                // Eğer yeterli stok yoksa stok yetersiz eventini tetikler
                await _publishEndpoint.Publish(new StockNotReservedEvent(context.Message.CorrelationId)
                {
                    Reason = "Not enough stock"
                });

                _logger.LogInformation($"Not enough stock for Buyer Id :{context.Message.CorrelationId}");
            }
        }
    }
}