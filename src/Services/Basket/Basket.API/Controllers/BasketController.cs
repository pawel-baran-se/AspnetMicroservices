using Basket.API.Entities;
using Basket.API.Repositories;
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

		public BasketController(ILogger<BasketController> logger, IBasketRepository basketRepository)
		{
			_logger = logger;
			_basketRepository = basketRepository;
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
			return Ok(await _basketRepository.UpdateBasket(basket));
		}

		[HttpDelete("{userName}", Name = "DeleteBasket")]
		[ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> DeleteBasket(string userName)
		{
			await _basketRepository.DeleteBasket(userName);

			return Ok();
		}

	}
}
