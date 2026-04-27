namespace RetailOrderingSystem.DTOs.Cart
{
    public class CartDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }

    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
