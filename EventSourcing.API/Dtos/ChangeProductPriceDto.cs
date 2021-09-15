using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcing.API.Dtos
{
    public class ChangeProductPriceDto
    {
        public Guid Id { get; set; }

        public decimal Price { get; set; }
    }
}
