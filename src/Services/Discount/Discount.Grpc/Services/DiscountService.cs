using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;

namespace Discount.Grpc.Services
{
	public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
	{
		private readonly ILogger<DiscountService> _logger;
		private readonly IDiscountRepository _repository;

		public DiscountService(ILogger<DiscountService>  logger,IDiscountRepository repository)
		{
			_logger = logger;
			_repository = repository;
		}
	}
}
