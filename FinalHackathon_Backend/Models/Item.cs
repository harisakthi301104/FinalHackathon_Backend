using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalHackathon_Backend.Models
{
    public class Item
    {

        [Key]
        public int ItemId { get; set; }

        // Name of the menu item
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Description of the item
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        // Price of the item
        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        // Number of units in stock
        [Required]
        public int StockQuantity { get; set; }

        // URL for item image
        public string ImageUrl { get; set; } = string.Empty;

        // Whether the item is available for ordering
        public bool IsAvailable { get; set; } = true;

        // Foreign key to Category
        [Required]
        public int CategoryId { get; set; }

        // Navigation: parent category
        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        // When the item was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
