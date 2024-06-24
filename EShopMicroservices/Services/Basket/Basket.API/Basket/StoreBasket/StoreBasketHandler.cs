using Discount.Grpc;
using System.Threading;

namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(ShoppingCart Cart):ICommand<StoreBasketResult>;
    public record StoreBasketResult(string UserName);
    public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketCommandValidator()
        {
            RuleFor(x => x.Cart).NotNull().WithMessage("Cart can not be null");
            RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required");
        }
    }
    public class StoreBasketHandler : ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {
        public readonly IBasketRepository _repository;
        public DiscountProtoService.DiscountProtoServiceClient _discountProto;
        public StoreBasketHandler(IBasketRepository repository, DiscountProtoService.DiscountProtoServiceClient discountProto)
        {
            _repository = repository;
            _discountProto = discountProto;
        }
        public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart cart = command.Cart;
            await DeductDiscount(cart, cancellationToken);
            // TODO: store basket in database(use Marten upsert -if exist = update,if not exist = insert)
            await _repository.StoreBasket(cart, cancellationToken);
            // TODO: update cache
            return new StoreBasketResult(cart.UserName);
        }

        private async Task DeductDiscount(ShoppingCart cart,CancellationToken cancellationToken)
        {
            // Communicate with Discount.Grpc and calculate lastest prices of products into sc
            foreach (var item in cart.Items)
            {
                var coupon = await _discountProto.GetDiscountAsync(new GetDiscountRequest { ProductName = item.ProductName }, cancellationToken: cancellationToken);
                item.Price -= coupon.Amount;
            }
        }
    }
}
