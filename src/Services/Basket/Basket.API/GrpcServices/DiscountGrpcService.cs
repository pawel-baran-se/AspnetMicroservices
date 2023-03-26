using Discount.Grpc.Protos;

namespace Basket.API.GrpcServices
{
	public class DiscountGrpcService
	{
		private readonly DiscountProtoService.DiscountProtoServiceClient _discountService;

		public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient service)
		{
			_discountService = service;
		}

		public async Task<CouponModel> GetDiscount(string productName)
		{
			var discountRequest = new GetDiscountRequest { ProductName = productName };

			return await _discountService.GetDiscountAsync(discountRequest);
		}
	}
}
