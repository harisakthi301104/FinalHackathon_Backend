using RetailOrderingSystem.DTOs.Order;

namespace RetailOrderingSystem.Services
{
    public interface IOrderService
    {
        Task<OrderDto> PlaceOrderAsync(int userId, PlaceOrderDto dto);
        Task<OrderDto> GetOrderAsync(int userId, int orderId);
    }

    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDto> PlaceOrderAsync(int userId, PlaceOrderDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Step 1: Get user's cart with items
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Item)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || !cart.CartItems.Any())
                    throw new InvalidOperationException("Cart is empty");

                // Step 2: Validate stock for each item and calculate total
                decimal totalAmount = 0;
                foreach (var cartItem in cart.CartItems)
                {
                    var item = cartItem.Item;

                    // Check if item still available
                    if (!item.IsAvailable)
                        throw new InvalidOperationException($"Item '{item.Name}' is no longer available");

                    // Check sufficient stock
                    if (item.StockQuantity < cartItem.Quantity)
                        throw new InvalidOperationException(
                            $"Insufficient stock for '{item.Name}'. Available: {item.StockQuantity}, Requested: {cartItem.Quantity}");

                    // Calculate line total (use current item price)
                    totalAmount += item.Price * cartItem.Quantity;
                }

                // Step 3: Create Order
                var order = new Order
                {
                    UserId = userId,
                    TotalAmount = totalAmount,
                    Status = "PENDING",
                    OrderDate = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Step 4: Create OrderItems and update stock
                foreach (var cartItem in cart.CartItems)
                {
                    var item = cartItem.Item;

                    // Create OrderItem (snapshot of price at time of order)
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ItemId = item.ItemId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = item.Price // Snapshot current price
                    };

                    _context.OrderItems.Add(orderItem);

                    // Reduce item stock
                    item.StockQuantity -= cartItem.Quantity;

                    // Mark as unavailable if stock reaches 0
                    if (item.StockQuantity <= 0)
                        item.IsAvailable = false;
                }

                await _context.SaveChangesAsync();

                // Step 5: Clear the cart
                _context.CartItems.RemoveRange(cart.CartItems);
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Step 6: Commit transaction
                await transaction.CommitAsync();

                // Reload order with items for response
                var createdOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstAsync(o => o.OrderId == order.OrderId);

                return MapToOrderDto(createdOrder);
            }
            catch (Exception)
            {
                // Rollback on any error
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Retrieves order details for a specific user and order ID
        /// Ensures user can only view their own orders
        /// </summary>
        public async Task<OrderDto> GetOrderAsync(int userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
                throw new InvalidOperationException("Order not found or access denied");

            return MapToOrderDto(order);
        }

        /// <summary>
        /// Maps Order model to OrderDto
        /// </summary>
        private static OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    ItemId = oi.ItemId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
    }
}
