using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stock.API.Services.Abstract
{
    public interface IStockService
    {
        Task<List<Models.Stock>> GetStocks();
    }
}
