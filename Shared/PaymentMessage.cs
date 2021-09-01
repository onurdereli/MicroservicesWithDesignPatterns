﻿using System;

namespace Shared
{
    public class PaymentMessage
    {
        public string CardName { get; set; }

        public string CardNumber { get; set; }

        public string Expiration { get; set; }

        public string Cvv { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
