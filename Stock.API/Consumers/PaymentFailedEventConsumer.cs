using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Events;
using Shared.Events.Abstract;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<IPaymentFailedEvent>
    {
        private readonly StockDbContext _stockDbContext;
        private readonly ILogger<PaymentFailedEventConsumer> _logger;

        public PaymentFailedEventConsumer(StockDbContext stockDbContext, ILogger<PaymentFailedEventConsumer> logger)
        {
            _stockDbContext = stockDbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IPaymentFailedEvent> context)
        {
            // Ödeme hatalı olduğunda stok için ayrılan ürün sayıları tekrar geri eklenir
            foreach (var item in context.Message.OrderItems)
            {
                var stock = await _stockDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                if (stock != null)
                {
                    stock.Count += item.Count;
                    await _stockDbContext.SaveChangesAsync();
                }
            }

            _logger.LogInformation($"Stock was released for Order Id ({context.Message.OrderId})");
        }
    }
}
