﻿namespace Ordering.Application.Orders.Queries.GetOrdersByName
{
    public class GetOrdersByNameHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetOrdersByNameQuery, GetOrdersByNameResult>
    {
        public async Task<GetOrdersByNameResult> Handle(GetOrdersByNameQuery query, CancellationToken cancellationToken)
        {
            // get orders by name using dbContext
            var orders = await dbContext.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.OrderName.Value.Contains(query.Name))
                .OrderBy(o => o.OrderName.Value)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // return result
            return new GetOrdersByNameResult(orders.ToOrderDtoList());
        }
    }
}
