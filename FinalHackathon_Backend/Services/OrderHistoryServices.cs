using FinalHackathon_Backend.Data;
using FinalHackathon_Backend.DTO;
using Microsoft.EntityFrameworkCore;

namespace FinalHackathon_Backend.Services
{
    public class OrderHistoryService
    {
        private readonly AppDbContext _context;

        public OrderHistoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderHistoryDto>> GetUserOrderHistoryAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => new OrderHistoryDto
            {
                OrderId = o.OrderId,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                OrderDate = o.OrderDate,
                Items = o.OrderItems.Select(oi => new OrderHistoryItemDto
                {
                    ItemId = oi.ItemId,
                    ItemName = oi.Item.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            }).ToList();
        }

        public async Task<OrderHistoryDto> GetOrderDetailAsync(int orderId, int userId)
        {
            var order = await _context.Orders
                .Where(o => o.OrderId == orderId && o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .FirstOrDefaultAsync();

            if (order == null)
                return null;

            return new OrderHistoryDto
            {
                OrderId = order.OrderId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                Items = order.OrderItems.Select(oi => new OrderHistoryItemDto
                {
                    ItemId = oi.ItemId,
                    ItemName = oi.Item.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
    }

}
