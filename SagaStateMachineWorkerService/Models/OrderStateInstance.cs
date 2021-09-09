using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Automatonymous;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateInstance : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }

        public string BuyerId { get; set; }

        public int OrderId { get; set; }

        public string CardName { get; set; }

        public string CardNumber { get; set; }

        public string Expiration { get; set; }

        public string Cvv { get; set; }
        
        public decimal TotalPrice { get; set; }

        public DateTime CreatedDate { get; set; }

        public override string ToString()
        {
            var properties = GetType().GetProperties();

            StringBuilder stringBuilder = new();

            properties.ToList().ForEach(p =>
            {
                var value = p.GetValue(this, null);
                stringBuilder.Append($"{p.Name}:{value}");
            });

            stringBuilder.Append("------------");

            return stringBuilder.ToString();
        }
    }
}
