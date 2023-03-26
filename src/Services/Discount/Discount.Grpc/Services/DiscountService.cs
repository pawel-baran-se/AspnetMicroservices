using AutoMapper;
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

		public DiscountService(ILogger<DiscountService>  logger,IDiscountRepository repository, IMapper mapper)
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
			var couponModel = _mapper.Map<CouponModel>(coupon);
			return couponModel;
		}
	}
}
