using System;
using Discount.Grpc;

namespace Basket.API.GrpcServices
{
	public class DiscountGrpcServices
	{
		private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoService;

        public DiscountGrpcServices(DiscountProtoService.DiscountProtoServiceClient discountProtoService)
        {
            _discountProtoService = discountProtoService ?? throw new ArgumentNullException(nameof(discountProtoService));
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            var discountRequest = new GetDiscountRequest { ProductName = productName };

            return await _discountProtoService.GetDiscountAsync(discountRequest);
        }

        //public async Task<CouponModel> CreateDiscount(CouponModel couponModel)
        //{
        //    var discountRequest = new CreateDiscountRequest { Coupon = couponModel};

        //    return await _discountProtoService.CreateDiscountAsync(discountRequest);
        //}
    }
}

