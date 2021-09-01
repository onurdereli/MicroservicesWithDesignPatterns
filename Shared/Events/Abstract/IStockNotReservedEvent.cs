namespace Shared.Events.Abstract
{
    public interface IStockNotReservedEvent
    {
        public int OrderId { get; set; }

        public string Message { get; set; }
    }
}
