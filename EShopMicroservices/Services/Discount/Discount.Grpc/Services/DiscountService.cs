using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services
{
    public class DiscountService:DiscountProtoService.DiscountProtoServiceBase
    {
        public readonly DiscountContext _dbContext;
        public readonly ILogger<DiscountService> _logger;
        public DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            // TODO: GetDiscount from Database
            var coupon = await _dbContext.Coupons
                .FirstOrDefaultAsync(x => x.ProductName == request.ProductName);
            if (coupon is null)
            {
                coupon = new Coupon
                {
                    ProductName = "No Discount",
                    Amount = 0,
                    Description = "No Discount Desc"
                };
            }

            _logger.LogInformation($"Discount is retrieved for ProductName:{coupon.ProductName},Amount:{coupon.Amount},Description:{coupon.Description}");

            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Coupon>();
            if (coupon is null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));
            }

            _dbContext.Coupons.Add(coupon);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Discount is successfully created.ProductName:{coupon.ProductName}");

            var couponModel = coupon.Adapt<CouponModel>();

            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Coupon>();
            if (coupon is null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));
            }

            _dbContext.Coupons.Update(coupon);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Discount is successfully updated.ProductName:{coupon.ProductName}");

            var couponModel = coupon.Adapt<CouponModel>();

            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDisountRequest request, ServerCallContext context)
        {
            var coupon = await _dbContext.Coupons
                .FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

            if (coupon is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found."));
            }

            _dbContext.Coupons.Remove(coupon);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Discount is successfully deleted.ProductName:{coupon.ProductName}");
            return new DeleteDiscountResponse { Success = true };
        }
    }
}
