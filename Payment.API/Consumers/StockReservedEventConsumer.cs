using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Events.Abstract;
using Shared.Events.Concrete;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer: IConsumer<IStockReservedEvent>
    {
        private readonly ILogger<StockReservedEventConsumer> _logger;

        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IStockReservedEvent> context)
        {
            //payment işlemleri için test olarak 3000 birim belirlenmiştir
            var balance = 3000m;

            if (balance > context.Message.Payment.TotalPrice)
            {
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was withdrawn from credit card for user id= {context.Message.BuyerId}");
                //Ödeme başarılı sayıldıysa tamamlandı eventi atar
                await _publishEndpoint.Publish(new PaymentCompletedEvent { BuyerId = context.Message.BuyerId, OrderId = context.Message.OrderId });
            }
            else
            {
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was not withdrawn from credit card for user id={context.Message.BuyerId}");
                //Ödeme yetersiz bakiyeden dolayı hata eventi atar
                await _publishEndpoint.Publish(new PaymentFailedEvent { BuyerId = context.Message.BuyerId, OrderId = context.Message.OrderId, Message = "not enough balance", OrderItems = context.Message.OrderItems });
            }
        }
    }
}
