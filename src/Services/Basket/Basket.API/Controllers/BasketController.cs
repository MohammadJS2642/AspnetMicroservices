using Basket.API.Entities;
using Basket.API.GrpcServies;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {

        private readonly IBasketRepository _repository;
        private readonly GrpcDiscountService _grpcDiscountService;

        public BasketController(IBasketRepository repository, GrpcDiscountService grpcDiscountService)
        {
            _repository = repository;
            _grpcDiscountService = grpcDiscountService;
        }

        [HttpGet("{username}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
        {
            var basket = await _repository.GetBasket(username);
            return Ok(basket);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart shoppingCart)
        {

            shoppingCart.ShoppingCartItems.ForEach(async item =>
            {
                var coupon = await _grpcDiscountService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            });

            var basket = await _repository.UpdateBasket(shoppingCart);
            return Ok(basket);
        }

        [HttpDelete("{username}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteBasket(string username)
        {
            await _repository.DeleteBasket(username);
            return Ok();
        }

    }
}
