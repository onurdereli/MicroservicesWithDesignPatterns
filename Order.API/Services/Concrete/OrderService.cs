using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Order.API.Dtos;
using Order.API.Enums;
using Order.API.Models;
using Order.API.Services.Abstract;
using Shared;
using Shared.Events.Abstract;
using Shared.Events.Concrete;

namespace Order.API.Services.Concrete
{
    public class OrderService :IOrderService
    {
        private readonly OrderDbContext _orderDbContext;
        
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public OrderService(OrderDbContext orderDbContext, ISendEndpointProvider sendEndpointProvider)
        {
            _orderDbContext = orderDbContext;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task<int> CreateOrder(OrderCreateDto orderCreate)
        {
            var newOrder = new Models.Order
            {
                BuyerId = orderCreate.BuyerId,
                Status = OrderStatus.Suspend,
                Address = new Address { Line = orderCreate.Address.Line, Province = orderCreate.Address.Province, District = orderCreate.Address.District },
                CreatedDate = DateTime.Now
            };

            orderCreate.OrderItems.ForEach(item =>
            {
                newOrder.Items.Add(new OrderItem() { Price = item.Price, ProductId = item.ProductId, Count = item.Count });
            });
            
            await _orderDbContext.AddAsync(newOrder);

            await _orderDbContext.SaveChangesAsync();

            var orderCreatedRequestEvent = new OrderCreatedRequestEvent()
            {
                BuyerId = orderCreate.BuyerId,
                OrderId = newOrder.Id,
                Payment = new PaymentMessage
                {
                    CardName = orderCreate.Payment.CardName,
                    CardNumber = orderCreate.Payment.CardNumber,
                    Expiration = orderCreate.Payment.Expiration,
                    Cvv = orderCreate.Payment.Cvv,
                    TotalPrice = orderCreate.OrderItems.Sum(x => x.Price * x.Count)
                },
            };

            orderCreate.OrderItems.ForEach(item =>
            {
                orderCreatedRequestEvent.OrderItems.Add(new OrderItemMessage { Count = item.Count, ProductId = item.ProductId });
            });

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMqSettingsConst.OrderSaga}"));

            await sendEndpoint.Send<IOrderCreatedRequestEvent>(orderCreatedRequestEvent);

            return 1;
        }
    }
}
