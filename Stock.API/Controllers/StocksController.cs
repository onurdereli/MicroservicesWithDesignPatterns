using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Stock.API.Services.Abstract;

namespace Stock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StocksController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _stockService.GetStocks());
        }
    }
}
