using System.ComponentModel.DataAnnotations;

namespace FOBackend.DTOs.Menu
{
    public class UpdateItemDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        [Required]
        public int CategoryId { get; set; }
    }
}
