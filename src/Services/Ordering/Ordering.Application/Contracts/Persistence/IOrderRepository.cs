﻿namespace Ordering.Application.Contracts.Persistence
{
    using Ordering.Domain.Entities;

    public interface IOrderRepository : IAsyncRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUsername(string userName);
    }
}
