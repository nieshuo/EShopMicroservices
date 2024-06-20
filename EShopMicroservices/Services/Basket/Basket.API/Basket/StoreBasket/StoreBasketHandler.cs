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
        public StoreBasketHandler(IBasketRepository repository)
        {
            _repository = repository;
        }
        public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart cart = command.Cart;
            // TODO: store basket in database(use Marten upsert -if exist = update,if not exist = insert)
            await _repository.StoreBasket(cart, cancellationToken);
            // TODO: update cache
            return new StoreBasketResult(cart.UserName);
        }
    }
}
