using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = Serilog.ILogger;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly ISerializeService _serializeService;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger _logger;

        public BasketRepository(ISerializeService serializeService, IDistributedCache distributedCache, ILogger logger)
        {
            _serializeService = serializeService;
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<bool> DeleteBasketFromUserName(string userName)
        {
            try
            {
                _logger.Information($"START: DeleteBasketFromUserName on {userName}");
                await _distributedCache.RemoveAsync(userName);
                _logger.Information($"END: DeleteBasketFromUserName on {userName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("DeleteBasketFromUserName" + ex.Message);
                throw;
            }
        }

        public async Task<Cart?> GetBasketByUserName(string userName)
        {
            _logger.Information("START: GetBasketByUserName on " + userName);
            var basket = await _distributedCache.GetStringAsync(userName);
            _logger.Information("END: GetBasketByUserName on " + userName);
            return !string.IsNullOrEmpty(basket) ? _serializeService.Deserialize<Cart>(basket) : null;
        }

        public async Task<Cart?> UpdateBasket(Cart cart, DistributedCacheEntryOptions options = null)
        {
            _logger.Information($"START: UpdateBasket on {cart.UserName}");
            if (options != null)
            {
                await _distributedCache.SetStringAsync(cart.UserName, _serializeService.Serialize(cart), options);
            }
            else
            {
                await _distributedCache.SetStringAsync(cart.UserName, _serializeService.Serialize(cart));
            }
            _logger.Information($"END: UpdateBasket on {cart.UserName}");

            return await GetBasketByUserName(cart.UserName);
        }
    }
}
