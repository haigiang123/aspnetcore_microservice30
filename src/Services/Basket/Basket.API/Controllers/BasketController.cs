using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;

        public BasketController(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
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
    }
}
