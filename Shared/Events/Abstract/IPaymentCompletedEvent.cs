namespace Shared.Events.Abstract
{
    public interface IPaymentCompletedEvent
    {
        public int OrderId { get; set; }

        public string BuyerId { get; set; }
    }
}
