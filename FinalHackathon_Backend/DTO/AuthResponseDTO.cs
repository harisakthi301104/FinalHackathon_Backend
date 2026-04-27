namespace FinalHackathon_Backend.DTO
{
    public class AuthResponseDto
    {
        // JWT token string
        public string Token { get; set; } = string.Empty;

        // User's role (ADMIN or USER)
        public string Role { get; set; } = string.Empty;

        // User's full name
        public string FullName { get; set; } = string.Empty;
    }
}
