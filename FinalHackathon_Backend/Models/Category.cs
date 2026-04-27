using System.ComponentModel.DataAnnotations;

namespace FinalHackathon_Backend.Models
{
    // Category entity to group menu items
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        // Name of the category (e.g., "Pizza", "Drinks")
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Short description of this category
        [MaxLength(300)]
        public string Description { get; set; } = string.Empty;

        // When the category was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation: items in this category
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
