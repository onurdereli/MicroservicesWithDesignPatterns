using System.Collections.Generic;
using Shared.Messages.Abstract;

namespace Shared.Messages.Concrete
{
    public class StockRollbackMessage: IStockRollbackMessage
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
