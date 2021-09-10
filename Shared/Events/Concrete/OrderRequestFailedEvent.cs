using Shared.Events.Abstract;

namespace Shared.Events.Concrete
{
    public class OrderRequestFailedEvent: IOrderRequestFailedEvent
    {
        public int OrderId { get; set; }

        public string Reason { get; set; }
    }
}
