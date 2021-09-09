using System;
using Automatonymous;
using Shared.Events.Abstract;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; }

        public State OrderCreated { get; set; }

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
                    .TransitionTo(OrderCreated)
                    .Then(context =>
                    {
                        Console.WriteLine($"OrderCreatedRequestEvent after: {context.Instance}");
                    })
                );
        }
    }
}
