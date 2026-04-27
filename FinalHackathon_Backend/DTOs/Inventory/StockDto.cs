namespace FOBackend.DTOs.Inventory;

public class StockDto
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsAvailable { get; set; }
    public string ImageUrl { get; set; }
}
