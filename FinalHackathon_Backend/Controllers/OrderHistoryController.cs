using FinalHackathon_Backend.DTO;
using FinalHackathon_Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalHackathon_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderHistoryController : ControllerBase
    {
        private readonly OrderHistoryService _orderHistoryService;

        public OrderHistoryController(OrderHistoryService orderHistoryService)
        {
            _orderHistoryService = orderHistoryService;
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var userId) ? userId : null;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderHistoryDto>>> GetOrderHistory()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Invalid user token" });

            var orders = await _orderHistoryService.GetUserOrderHistoryAsync(userId.Value);
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderHistoryDto>> GetOrderDetail(int orderId)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Invalid user token" });

            var order = await _orderHistoryService.GetOrderDetailAsync(orderId, userId.Value);

            if (order == null)
                return NotFound(new { message = "Order not found or does not belong to this user" });

            return Ok(order);
        }
    }
}
