using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services
{
	public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
	{
		private readonly ILogger<DiscountService> _logger;
		private readonly IDiscountRepository _repository;
		private readonly IMapper _mapper;

		public DiscountService(ILogger<DiscountService> logger, IDiscountRepository repository, IMapper mapper)
		{
			_logger = logger;
			_repository = repository;
			_mapper = mapper;
		}

		public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
		{
			var coupon = await _repository.GetDiscount(request.ProductName);
			if (coupon == null)
			{
				throw new RpcException(new Status(StatusCode.NotFound, $"Discount with product name = {request.ProductName} was not found."));
			}
			_logger.LogInformation($"Discount is retrieved for ProductName: {coupon.ProductName}, with amount {coupon.Amount}");
			var couponModel = _mapper.Map<CouponModel>(coupon);
			return couponModel;
		}

		public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
		{
			var coupon = _mapper.Map<Coupon>(request.Coupon);
			await _repository.CreateDiscount(coupon);

			_logger.LogInformation("Discount is successfully created: {ProductName}", coupon.ProductName);

			var couponModel = _mapper.Map<CouponModel>(coupon);
			return couponModel;
		}

		public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
		{
			var coupon = _mapper.Map<Coupon>(request.Coupon);
			await _repository.UpdateDiscount(coupon);

			_logger.LogInformation("Discount is successfully updated: {ProductName}", coupon.ProductName);

			var couponModel = _mapper.Map<CouponModel>(coupon);
			return couponModel;
		}

		public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
		{
			var deleted = await _repository.DeleteDiscount(request.ProductName);
			_logger.LogInformation("{ProductName} discount deleted successfully", request.ProductName);
			var response = new DeleteDiscountResponse
			{
				Success = deleted
			};

			return response;
		}
	}
}
