using FinalHackathon_Backend.Data;
using FinalHackathon_Backend.DTO;
using FinalHackathon_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinalHackathon_Backend.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthService(AppDbContext context, JwtSettings jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings;
        }

        // Register a new user with hashed password
        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            // Check if email already exists
            var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                return "Email already registered";

            // Create new user with hashed password
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "USER",
                CreatedAt = DateTime.UtcNow
            };

            // Save to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "Success";
        }

        // Login user and return JWT token
        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return null;

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            // Generate JWT token
            var token = GenerateJwtToken(user);

            // Return response with token and user info
            return new AuthResponseDto
            {
                Token = token,
                Role = user.Role,
                FullName = user.FullName
            };
        }

        // Generate a JWT token with user claims
        private string GenerateJwtToken(User user)
        {
            // Create signing key from secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Add claims: userId, email, role
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            // Build the token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: credentials
            );

            // Return token as string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
}
