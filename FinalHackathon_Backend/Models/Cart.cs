using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailOrderingSystem.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        // Foreign key to User
        [Required]
        public int UserId { get; set; }

        // Navigation: the user who owns this cart
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        // When the cart was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // When the cart was last updated
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation: items in this cart
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

}
