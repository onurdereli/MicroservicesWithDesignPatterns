using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Order.API.Dtos;

namespace Order.API.Services.Abstract
{
    public interface IOrderService
    {
        Task<int> CreateOrder(OrderCreateDto orderCreateDto);
    }
}
