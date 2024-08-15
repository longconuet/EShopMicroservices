using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services;

public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
    private readonly DiscountDbContext _dbContext;
    private readonly ILogger<DiscountService> _logger;

    public DiscountService(DiscountDbContext dbContext, ILogger<DiscountService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await _dbContext
            .Coupons
            .FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

        coupon ??= new Coupon
            {
                ProductName = "No discount",
                Description = "No discount",
                Amount = 0,
            };

        _logger.LogInformation($"Discount is retrieved for ProductName: {coupon.ProductName}, Amount: {coupon.Amount}");

        return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>();
        if (coupon is null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request create discount"));
        }

        _dbContext.Add(coupon);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Created new discount: {coupon.ProductName}");

        return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>();
        if (coupon is null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request create discount"));
        }

        _dbContext.Update(coupon);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Updated discount: {coupon.ProductName}");

        return coupon.Adapt<CouponModel>();
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var coupon = await _dbContext
            .Coupons
            .FirstOrDefaultAsync(x => x.ProductName == request.ProductName);
        if (coupon is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Discount not found: {request.ProductName}"));
        }

        _dbContext.Remove(coupon);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Deleted discount: {coupon.ProductName}");

        return new DeleteDiscountResponse { Success = true };
    }
}
