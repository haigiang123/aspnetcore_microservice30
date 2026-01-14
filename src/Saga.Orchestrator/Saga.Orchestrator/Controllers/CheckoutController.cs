using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOs.Basket;
using System.ComponentModel.DataAnnotations;

namespace Saga.Orchestrator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutSagaService _checkoutSagaService;

        public CheckoutController(ICheckoutSagaService checkoutSagaService)
        {
            _checkoutSagaService = checkoutSagaService;
        }

        [HttpPost]
        [Route("username")]
        public async Task<IActionResult> CheckoutOrder([Required] string username, [FromBody] BasketCheckoutDto model)
        {
            var result = await _checkoutSagaService.CheckoutOrder(username, model);
            return Ok(result);
        }
    }
}
