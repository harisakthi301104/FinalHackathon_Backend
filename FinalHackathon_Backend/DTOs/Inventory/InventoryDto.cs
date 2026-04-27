namespace FOBackend.DTOs.Inventory
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
