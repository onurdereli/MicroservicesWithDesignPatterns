using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcing.API.Dtos
{
    public class ChangeProductNameDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
