using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalHackathon_Backend.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        // Foreign key to Order
        [Required]
        public int OrderId { get; set; }

        // Navigation: parent order
        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;

        // Foreign key to Item
        [Required]
        public int ItemId { get; set; }

        // Navigation: the menu item
        [ForeignKey("ItemId")]
        public Item Item { get; set; } = null!;

        // Quantity ordered
        [Required]
        public int Quantity { get; set; }

        // Price at the time of ordering (snapshot)
        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }
    }
}
