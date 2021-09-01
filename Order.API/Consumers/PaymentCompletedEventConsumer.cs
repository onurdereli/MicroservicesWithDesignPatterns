using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Order.API.Enums;
using Order.API.Models;
using Shared.Events.Abstract;

namespace Order.API.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<IPaymentCompletedEvent>
    {
        private readonly OrderDbContext _orderDbContext;
        private readonly ILogger<PaymentCompletedEventConsumer> _logger;

        public PaymentCompletedEventConsumer(ILogger<PaymentCompletedEventConsumer> logger, OrderDbContext orderDbContext)
        {
            _logger = logger;
            _orderDbContext = orderDbContext;
        }

        public async Task Consume(ConsumeContext<IPaymentCompletedEvent> context)
        {
            var order = await _orderDbContext.Orders.FindAsync(context.Message.OrderId);

            if (order != null)
            {
                order.Status = OrderStatus.Complete;
                await _orderDbContext.SaveChangesAsync();

                _logger.LogInformation($"Order (Id: {context.Message.OrderId}) status changed: {order.Status}");

            }
            else
            {
                _logger.LogError($"Order (Id: {context.Message.OrderId}) not found");
            }
        }
    }
}
