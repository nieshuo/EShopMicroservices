namespace Ordering.Application.Orders.Queries.GetOrdersByCustomer
{
    internal class GetOrderByCustomerHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetOrdersByCustomerQuery, GetOrdersByCustomerResult>
    {
        public async Task<GetOrdersByCustomerResult> Handle(GetOrdersByCustomerQuery query, CancellationToken cancellationToken)
        {
            // get orders by customer using dbContext
            var orders = await dbContext.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == CustomerId.Of(query.CustomerId))
                .OrderBy(o => o.OrderName.Value)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // return result
            return new GetOrdersByCustomerResult(orders.ToOrderDtoList());
        }
    }
}
