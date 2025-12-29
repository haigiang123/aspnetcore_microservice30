using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository, IPublishEndpoint publishEndpoint, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
        }

        [HttpGet(template: "{userName}", Name = "GetBasket")]
        public async Task<ActionResult<Cart>> GetBasketByUserName(string userName)
        {
            return Ok(await _basketRepository.GetBasketByUserName(userName) ?? new Cart());
        }

        [HttpDelete(template: "{userName}", Name = "DeleteBasket")]
        public async Task<ActionResult<Boolean>> DeleteBasket(string userName)
        {
            return Ok(await _basketRepository.DeleteBasketFromUserName(userName));
        }

        [HttpPost(Name = "UpdateBasket")]
        public async Task<ActionResult<Cart>> UpdateBasket(Cart cart)
        {
            var result = await _basketRepository.UpdateBasket(cart, new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });

            return Ok(result);
        }

        [Route("[action]/{username}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([Required] string username, [FromBody] BasketCheckout basketCheckout)
        {
            var basket = await _basketRepository.GetBasketByUserName(username);
            if (basket == null || !basket.Items.Any()) return NotFound();

            //publish checkout event to EventBus Message
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMessage);

            return Accepted();
        }
    }
}
