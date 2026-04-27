using System.ComponentModel.DataAnnotations;

namespace FinalHackathon_Backend.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        // Full name of the user
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        // Email used for login (must be unique)
        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        // Hashed password stored securely
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Role: "ADMIN" or "USER"
        [Required, MaxLength(20)]
        public string Role { get; set; } = "USER";

        // When the user registered
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
