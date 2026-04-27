using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailOrderingSystem.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        // Foreign key to Cart
        [Required]
        public int CartId { get; set; }

        // Navigation: parent cart
        [ForeignKey("CartId")]
        public Cart Cart { get; set; } = null!;

        // Foreign key to Item
        [Required]
        public int ItemId { get; set; }

        // Navigation: the menu item
        [ForeignKey("ItemId")]
        public Item Item { get; set; } = null!;

        // How many of this item in the cart
        [Required]
        public int Quantity { get; set; }
    }
}
