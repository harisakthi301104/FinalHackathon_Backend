namespace RetailOrderingSystem.DTOs.Order
{
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
