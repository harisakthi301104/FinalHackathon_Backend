using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalHackathon_Backend.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // Foreign key to User who placed the order
        [Required]
        public int UserId { get; set; }

        // Navigation: the user who placed this order
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        // Total cost of the order
        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        // Status: PENDING, CONFIRMED
        [Required, MaxLength(20)]
        public string Status { get; set; } = "PENDING";

        // When the order was placed
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // Navigation: items in this order
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
