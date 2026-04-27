using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailOrderingSystem.DTOs.Order;
using RetailOrderingSystem.Services;

namespace RetailOrderingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Wait for Phase 0 Auth setup
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            // Parse from JWT Claims
            // return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return 1; // Placeholder
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
        {
            try
            {
                var order = await _orderService.PlaceOrderAsync(GetUserId(), dto);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _orderService.GetOrderAsync(GetUserId(), orderId);
            return Ok(order);
        }
    }
}
