using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServies;
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

        private readonly IBasketRepository _repository;
        private readonly GrpcDiscountService _grpcDiscountService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public BasketController(
            IBasketRepository repository,
            GrpcDiscountService grpcDiscountService,
            IPublishEndpoint publishEndpoint,
            IMapper mapper
        )
        {
            _repository = repository;
            _grpcDiscountService = grpcDiscountService;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
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

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            // get existing basket with total price
            // Create basketcheckoutEvent -- set TotalPrice on BasketCheckout eventMessage
            // send checkout event to rabbitmq
            // remove the basket

            // get existing basket with total price
            var basket = await _repository.GetBasket(basketCheckout.UserName);
            if (basket == null)
                return BadRequest();

            // send checkout event to rabbitmq
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMessage);

            // Remove the basket
            await _repository.DeleteBasket(basket.Username);

            return Accepted();
        }

    }
}
