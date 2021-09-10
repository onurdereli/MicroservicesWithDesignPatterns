using Shared.Events.Abstract;

namespace Shared.Events.Concrete
{
    public class OrderRequestCompletedEvent: IOrderRequestCompletedEvent
    {
        public int OrderId { get; set; }
    }
}
