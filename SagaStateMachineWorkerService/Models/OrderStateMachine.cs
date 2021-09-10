using System;
using Automatonymous;
using Shared;
using Shared.Events.Abstract;
using Shared.Events.Concrete;
using Shared.Messages.Concrete;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; }
        public Event<IStockReservedEvent> StockReservedEvent { get; set; }
        public Event<IStockNotReservedEvent> StockNotReservedEvent { get; set; }
        public Event<IPaymentCompletedEvent> PaymentCompletedEvent { get; set; }
        public Event<IPaymentFailedEvent> PaymentFailedEvent { get; set; }

        public State OrderCreated { get; set; }
        public State StockReserved { get; set; }
        public State StockNotReserved { get; set; }
        public State PaymentCompleted { get; set; }
        public State PaymentFailed { get; set; }

        //İlk eventte her zaman business işlemlerinden sonra initial state'i OrderCreated state'ine geçmesi için yapılan işlemler
        public OrderStateMachine()
        {
            // Initial'a current'i set eder
            InstanceState(x => x.CurrentState);

            // Aynı kayıt daha önceden yoksa yeni idli bi kayıt atması sağlanır
            // Dbdeki orderId'leri gelen mesajdaki orderId'lerle kıyaslamasını sağlar
            Event(() => OrderCreatedRequestEvent, y => y.CorrelateBy<int>(x => x.OrderId, z => z.Message.OrderId)
                .SelectId(context => Guid.NewGuid()));

            // StockReservedEvent'i dinleniyorsa veritabanındaki hangi satıra ait olduğunu alır
            Event(() => StockReservedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));

            // StockNotReservedEvent'i dinleniyorsa veritabanındaki hangi satıra ait olduğunu alır
            Event(() => StockNotReservedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));

            // PaymentCompletedEvent'i dinleniyorsa veritabanındaki hangi satıra ait olduğunu alır
            Event(() => PaymentCompletedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));

            // PaymentFailedEvent'i dinleniyorsa veritabanındaki hangi satıra ait olduğunu alır
            Event(() => PaymentFailedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));

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
                    })
                    .Then(context =>
                    {
                        Console.WriteLine($"StockReservedEvent After : {context.Instance}");
                    }),
                When(StockNotReservedEvent)

                    .TransitionTo(StockNotReserved)
                    .Publish(context => new OrderRequestFailedEvent { OrderId = context.Instance.OrderId, Reason = context.Data.Reason })
                    .Then(context =>
                    {
                        Console.WriteLine($"StockNotReservedEvent After : {context.Instance}");
                    })
                );

            //When; işlemi birden fazla koşul gerektiğinden koşulları belirtir
            //TransitionTo; bir sonraki state'i belirtir
            //publish ve send işlemlerinde send belirli bir queue'ya gönderme işlemleri için gerekli. Örn; ödeme işlemini tek bi queue dinlemesi gerektiğinden dolayı sadece send kullanılabilir
            //Finalize; Ödeme tamamlandı state'ine gelince Masstransitin saga implementasyonundaki default final adımına geçmesini sağlar
            During(StockReserved,
                When(PaymentCompletedEvent)
                    .TransitionTo(PaymentCompleted)
                    .Publish(context => new OrderRequestCompletedEvent { OrderId = context.Instance.OrderId })
                    .Then(context =>
                    {
                        Console.WriteLine($"PaymentCompletedEvent After : {context.Instance}");
                    })
                    .Finalize(),
                When(PaymentFailedEvent)
                    .Publish(context => new OrderRequestFailedEvent {OrderId = context.Instance.OrderId, Reason = context.Data.Reason})
                    .Send(new Uri($"queue:{RabbitMqSettingsConst.OrderRequestFailedEventQueueName}"), context => new StockRollbackMessage {OrderItems = context.Data.OrderItems})
                    .TransitionTo(PaymentFailed)
                    .Then(context =>
                    {
                        Console.WriteLine($"OrderRequestFailedEvent After : {context.Instance}");
                    })
            );

            // Bitmiş bütün mesajları temizler dbden siler
            SetCompletedWhenFinalized();
        }
    }
}
