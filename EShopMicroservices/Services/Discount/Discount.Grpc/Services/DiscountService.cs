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

        public override Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            return base.CreateDiscount(request, context);
        }

        public override Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            return base.UpdateDiscount(request, context);
        }

        public override Task<DeleteDiscountResponse> DeleteDiscount(DeleteDisountRequest request, ServerCallContext context)
        {
            return base.DeleteDiscount(request, context);
        }
    }
}
