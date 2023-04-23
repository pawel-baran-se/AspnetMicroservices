using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services;
using System.Net;

namespace Shopping.Aggregator.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class ShoppingController : ControllerBase
	{
		private readonly ICatalogService _catalogService;
		private readonly IBasketService _basketService;
		private readonly IOrderService _orderService;

		public ShoppingController(ICatalogService catalogService,
			IBasketService basketService,
			IOrderService orderService)
		{
			_catalogService = catalogService;
			_basketService = basketService;
			_orderService = orderService;
		}

		[HttpGet("{userName}", Name = "GetShopping")]
		[ProducesResponseType(typeof(ShoppingModel), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<ShoppingModel>> GetShopping(string userName)
		{
			//get basket with userName
			var basket = await _basketService.GetBasket(userName);

			//iterate the basket items and consume products with basket item product Id member
			foreach (var item in basket.Items)
			{
				var product = await _catalogService.GetCatalog(item.ProductId);

				item.ProductName = product.Name;
				item.Category = product.Category;
				item.Summary = product.Summary;
				item.Description = product.Description;
				item.ImageFile = product.ImageFile;
			}
			//map product related members into basketitem dto with extened column

			//consume ordering microservices in order to retrive order list
			var orders = await _orderService.GetOrdersByUserName(userName);
			//return root ShoppingModel dto class which includes all responsec
			var shoppingModel = new ShoppingModel
			{
				UserName = userName,
				BasketWithProducts = basket,
				Orders = orders
			};

			return Ok(shoppingModel);
		}
	}
}