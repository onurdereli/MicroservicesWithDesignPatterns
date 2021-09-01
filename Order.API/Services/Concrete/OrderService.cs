using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Order.API.Dtos;
using Order.API.Enums;
using Order.API.Models;
using Order.API.Services.Abstract;
using Shared;
using Shared.Events.Concrete;

namespace Order.API.Services.Concrete
{
    public class OrderService :IOrderService
    {
        private readonly OrderDbContext _orderDbContext;

        private readonly IPublishEndpoint _publishEndpoint;

        public OrderService(OrderDbContext orderDbContext, IPublishEndpoint publishEndpoint)
        {
            _orderDbContext = orderDbContext;
            _publishEndpoint = publishEndpoint;
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

            var orderCreatedEvent = new OrderCreatedEvent()
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
                orderCreatedEvent.OrderItems.Add(new OrderItemMessage { Count = item.Count, ProductId = item.ProductId });
            });

            await _publishEndpoint.Publish(orderCreatedEvent);

            return 1;
        }
    }
}
