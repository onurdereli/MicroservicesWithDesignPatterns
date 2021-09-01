using Shared.Events.Abstract;

namespace Shared.Events.Concrete
{
    public class StockNotReservedEvent : IStockNotReservedEvent
    {
        public int OrderId { get; set; }

        public string Message { get; set; }
    }
}
