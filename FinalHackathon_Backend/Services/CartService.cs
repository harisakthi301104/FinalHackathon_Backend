using RetailOrderingSystem.DTOs.Cart;
using RetailOrderingSystem.Models;

namespace RetailOrderingSystem.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);
        Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto);
        Task<CartDto> UpdateCartItemAsync(int userId, UpdateCartDto dto);
        Task<CartDto> RemoveFromCartAsync(int userId, int cartItemId);
    }

    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            // Get or create cart for the user
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // If no cart exists, create an empty one
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Map to DTO
            return MapToCartDto(cart);
        }

        /// <summary>
        /// Adds an item to the user's cart
        /// Updates quantity if item already exists in cart
        /// </summary>
        public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto)
        {
            // Validate item exists and is available
            var item = await _context.Items.FindAsync(dto.ItemId);
            if (item == null || !item.IsAvailable)
                throw new InvalidOperationException("Item not found or unavailable");

            // Check if requested quantity is available
            if (item.StockQuantity < dto.Quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {item.StockQuantity}");

            // Get or create cart
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Check if item already in cart
            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ItemId == dto.ItemId);

            if (existingCartItem != null)
            {
                // Update quantity
                existingCartItem.Quantity += dto.Quantity;

                // Verify total quantity doesn't exceed stock
                if (existingCartItem.Quantity > item.StockQuantity)
                    throw new InvalidOperationException($"Quantity exceeds available stock. Max available: {item.StockQuantity}");
            }
            else
            {
                // Add new cart item
                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ItemId = dto.ItemId,
                    Quantity = dto.Quantity
                };
                _context.CartItems.Add(cartItem);
            }

            // Update cart timestamp
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Reload cart with updated items
            cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstAsync(c => c.CartId == cart.CartId);

            return MapToCartDto(cart);
        }

        /// <summary>
        /// Updates the quantity of an existing cart item
        /// </summary>
        public async Task<CartDto> UpdateCartItemAsync(int userId, UpdateCartDto dto)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Item)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                throw new InvalidOperationException("Cart not found");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == dto.CartItemId);
            if (cartItem == null)
                throw new InvalidOperationException("Cart item not found");

            // Validate new quantity is positive
            if (dto.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than 0");

            // Check stock availability
            if (cartItem.Item.StockQuantity < dto.Quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {cartItem.Item.StockQuantity}");

            cartItem.Quantity = dto.Quantity;
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Reload cart
            cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstAsync(c => c.CartId == cart.CartId);

            return MapToCartDto(cart);
        }

        /// <summary>
        /// Removes a specific item from the cart
        /// </summary>
        public async Task<CartDto> RemoveFromCartAsync(int userId, int cartItemId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                throw new InvalidOperationException("Cart not found");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);
            if (cartItem == null)
                throw new InvalidOperationException("Cart item not found");

            _context.CartItems.Remove(cartItem);
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Reload cart
            cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstAsync(c => c.CartId == cart.CartId);

            return MapToCartDto(cart);
        }

        /// <summary>
        /// Clears all items from the user's cart
        /// </summary>
        public async Task<bool> ClearCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return false;

            _context.CartItems.RemoveRange(cart.CartItems);
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Maps Cart model to CartDto
        /// </summary>
        private static CartDto MapToCartDto(Cart cart)
        {
            var items = cart.CartItems.Select(ci => new CartItemDto
            {
                CartItemId = ci.CartItemId,
                ItemId = ci.ItemId,
                Quantity = ci.Quantity
            }).ToList();

            return new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                Items = items,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt
            };
        }
    }
}
