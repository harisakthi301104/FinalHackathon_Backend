namespace FinalHackathon_Backend.DTO
{
    public class OrderHistoryDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderHistoryItemDto> Items { get; set; }
    }

    public class OrderHistoryItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
