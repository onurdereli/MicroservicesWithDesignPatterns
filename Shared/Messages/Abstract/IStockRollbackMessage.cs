using System.Collections.Generic;

namespace Shared.Messages.Abstract
{
    public interface IStockRollbackMessage
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
