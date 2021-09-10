using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Messages.Abstract;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class StockRollbackMessageConsumer: IConsumer<IStockRollbackMessage>
    {
        private readonly StockDbContext _context;
        private readonly ILogger<StockRollbackMessageConsumer> _logger;

        public StockRollbackMessageConsumer(StockDbContext context, ILogger<StockRollbackMessageConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IStockRollbackMessage> context)
        {
            foreach (var item in context.Message.OrderItems)
            {
                var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                if (stock != null)
                {
                    stock.Count += item.Count;
                    await _context.SaveChangesAsync();
                }
            }

            _logger.LogInformation("Stock was released");
        }
    }
}
