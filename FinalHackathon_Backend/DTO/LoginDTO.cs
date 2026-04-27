using System.ComponentModel.DataAnnotations;

namespace FinalHackathon_Backend.DTO
{
    public class LoginDto
    {
        // User's email address
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        // User's password
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
