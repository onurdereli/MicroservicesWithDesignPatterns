﻿namespace Shared.Events.Abstract
{
    public interface IOrderRequestCompletedEvent
    {
        public int OrderId { get; set; }
    }
}
