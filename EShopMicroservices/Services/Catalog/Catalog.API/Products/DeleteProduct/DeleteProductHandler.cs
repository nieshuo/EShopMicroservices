
using Catalog.API.Products.GetProductByCategory;

namespace Catalog.API.Products.DeleteProduct
{
    public record DeleteProductCommond(Guid Id) : ICommand<DeleteProductResult>;
    public record DeleteProductResult(bool IsSuccess);
    internal class DeleteProductCommondHandler(IDocumentSession session, ILogger<GetProductByCategoryHandler> logger)
        : ICommandHandler<DeleteProductCommond, DeleteProductResult>
    {
        public async Task<DeleteProductResult> Handle(DeleteProductCommond commond, CancellationToken cancellationToken)
        {
            logger.LogInformation($"DeleteProductCommandHandler.Handle called with {commond}");
            session.Delete<Product>(commond.Id);

            await session.SaveChangesAsync(cancellationToken);

            return new DeleteProductResult(true);
        }
    }
}
