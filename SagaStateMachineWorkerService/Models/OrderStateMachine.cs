﻿using System;
using Automatonymous;
using Shared;
using Shared.Events.Abstract;
using Shared.Events.Concrete;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; }
        public Event<IStockReservedEvent> StockReservedEvent { get; set; }

        public State OrderCreated { get; set; }
        public State StockReserved { get; set; }

        //İlk eventte her zaman business işlemlerinden sonra initial state'i OrderCreated state'ine geçmesi için yapılan işlemler
        public OrderStateMachine()
        {
            // Initial'a current'i set eder
            InstanceState(x => x.CurrentState);

            // Aynı kayıt daha önceden yoksa yeni idli bi kayıt atması sağlanır
            // Dbdeki orderId'leri gelen mesajdaki orderId'lerle kıyaslamasını sağlar
            Event(() => OrderCreatedRequestEvent, y => y.CorrelateBy<int>(x => x.OrderId, z => z.Message.OrderId)
                .SelectId(context => Guid.NewGuid()));

            //OrderCreatedRequestEvent eventi tetiklendiğinde datalar ile dbye kayıt atılıp state'i orderCreated olacak
            Initially(
                When(OrderCreatedRequestEvent)
                    .Then(context =>
                    {
                        context.Instance.OrderId = context.Data.OrderId;
                        context.Instance.BuyerId = context.Data.BuyerId;

                        context.Instance.Cvv = context.Data.Payment.Cvv;
                        context.Instance.CardName = context.Data.Payment.CardName;
                        context.Instance.CardNumber = context.Data.Payment.CardNumber;
                        context.Instance.Expiration = context.Data.Payment.Expiration;
                        context.Instance.TotalPrice = context.Data.Payment.TotalPrice;

                        context.Instance.CreatedDate = DateTime.Now;
                    })
                    .Then(context =>
                    {
                        Console.WriteLine($"OrderCreatedRequestEvent before: {context.Instance}");
                    })
                    .Publish(context => new OrderCreatedEvent(context.Instance.CorrelationId) { OrderItems = context.Data.OrderItems })
                    .TransitionTo(OrderCreated)
                    .Then(context =>
                    {
                        Console.WriteLine($"OrderCreatedRequestEvent after: {context.Instance}");
                    })
                );

            During(OrderCreated,
                When(StockReservedEvent)
                    .Then(context =>
                    {
                        Console.WriteLine($"StockReservedEvent before: {context.Instance}");
                    })
                    .TransitionTo(StockReserved)
                    .Send(new Uri($"queue:{RabbitMqSettingsConst.PaymentStockReservedRequestQueueName }"), context => new StockReservedRequestPayment(context.Instance.CorrelationId)
                    {
                        OrderItems = context.Data.OrderItems,
                        Payment = new PaymentMessage()
                        {
                            CardName = context.Instance.CardName,
                            CardNumber = context.Instance.CardNumber,
                            Cvv = context.Instance.Cvv,
                            Expiration = context.Instance.Expiration,
                            TotalPrice = context.Instance.TotalPrice
                        },
                        BuyerId = context.Instance.BuyerId
                    }).Then(context => { Console.WriteLine($"StockReservedEvent After : {context.Instance}"); }));
        }
    }
}
