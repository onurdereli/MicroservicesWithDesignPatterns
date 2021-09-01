using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stock.API.Models;
using Stock.API.Services.Abstract;

namespace Stock.API.Services.Concrete
{
    public class StockService :IStockService
    {
        private readonly StockDbContext _stockDbContext;

        public StockService(StockDbContext stockDbContext)
        {
            _stockDbContext = stockDbContext;
        }

        public async Task<List<Models.Stock>> GetStocks()
        {
            return await _stockDbContext.Stocks.ToListAsync();
        }
    }
}
