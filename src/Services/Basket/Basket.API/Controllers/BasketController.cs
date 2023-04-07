using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class BasketController : ControllerBase
	{
		private readonly ILogger<BasketController> _logger;
		private readonly IBasketRepository _basketRepository;
		private readonly DiscountGrpcService _discountGrpcService;
		private readonly IMapper _mapper;
		private readonly IPublishEndpoint _publishEndpoint;

		public BasketController(ILogger<BasketController> logger,
			IBasketRepository basketRepository,
			DiscountGrpcService discountGrpcService,
			IMapper mapper,
			IPublishEndpoint publishEndpoint)
		{
			_logger = logger;
			_basketRepository = basketRepository;
			_discountGrpcService = discountGrpcService;
			_mapper = mapper;
			_publishEndpoint = publishEndpoint;
		}

		[HttpGet("{userName}", Name = "GetBasket")]
		[ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
		{
			var basket = await _basketRepository.GetBasket(userName);

			return Ok(basket ?? new ShoppingCart(userName));
		}

		[HttpPut(Name = "UpdateBasket")]
		[ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
		{
			//TODO: Communicate with Discount.Grpc
			//and calculate latest prices of the product into the shopping cart
			//consume gRpc
			foreach (var item in basket.Items)
			{
				var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
				item.Price -= coupon.Amount;
			}

			return Ok(await _basketRepository.UpdateBasket(basket));
		}

		[HttpDelete("{userName}", Name = "DeleteBasket")]
		[ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> DeleteBasket(string userName)
		{
			await _basketRepository.DeleteBasket(userName);

			return Ok();
		}

		[Route("[action]")]
		[HttpPost]
		[ProducesResponseType((int)HttpStatusCode.Accepted)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
		{
			//get existing basket
			var basket = await _basketRepository.GetBasket(basketCheckout.UserName);
			if (basket == null)
			{
				return BadRequest();
			}

			//set TotalPrice on basketCheckout
			var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
			//send checkout event to rabbitmq
			eventMessage.TotalPrice = basket.TotalPrice;
			await _publishEndpoint.Publish(eventMessage);

			//remove the basket
			await _basketRepository.DeleteBasket(basket.UserName);

			return Accepted();
		}
	}
}