using System;
using System.Collections.Generic;
using Order.API.Enums;

namespace Order.API.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string BuyerId { get; set; }

        public Address Address { get; set; }

        public ICollection<OrderItem> Items { get; set; }

        public OrderStatus Status { get; set; }

        public string FailMessage { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
        }
    }
}
