using BuildingBlocks.Messaging.Events;
using MassTransit;
using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.Application.Orders.EventHandlers.Integration
{
    public class BasketCheckoutEventHandler
        (ISender sender,ILogger<BasketCheckoutEventHandler> logger)
        : IConsumer<BasketCheckoutEvent>
    {
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            // TODO: Create new order and start order fullfillment process
            logger.LogInformation("Integration Event handled:{IntegrationEvent}", context.Message.GetType().Name);

            var command = MapToCreateOrderCommand(context.Message);
            await sender.Send(command);
        }

        private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutEvent messae)
        {
            // Create full order with incoming event data
            var addressDto = new AddressDto(
                messae.FirstName,
                messae.LastName,
                messae.EmailAddress,
                messae.AddressLine,
                messae.Country,
                messae.State,
                messae.ZipCode
            );

            var paymentDto = new PaymentDto(
                messae.CardName,
                messae.CardNumber,
                messae.Expiration,
                messae.CVV,
                messae.PaymentMethod
            );

            var orderId = Guid.NewGuid();

            var orderDto = new OrderDto(
                Id:orderId,
                CustomerId:messae.CustomerId,
                OrderName:messae.UserName,
                ShippingAddress:addressDto,
                BillingAddress:addressDto,
                Payment:paymentDto,
                Status:OrderStatus.Pending,
                OrderItems: [
                    new OrderItemDto(orderId,new Guid("5334C996-8457-4CF0-815C-ED2B77C4FF61"),2,500),
                    new OrderItemDto(orderId,new Guid("C67D6323-E8B1-4BDF-9A75-B0D0D2E7E914"),1,400)
                ]
            );

            return new CreateOrderCommand(orderDto);
        }
    }
}
