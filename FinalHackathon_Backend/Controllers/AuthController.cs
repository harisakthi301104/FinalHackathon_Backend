using FinalHackathon_Backend.DTO;
using FinalHackathon_Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalHackathon_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // POST /api/auth/register — Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Call auth service to register user
            var result = await _authService.RegisterAsync(dto);

            // Check if registration failed
            if (result != "Success")
                return BadRequest(new { message = result });

            // Return success message
            return Ok(new { message = "User registered successfully" });
        }

        // POST /api/auth/login — Login and get JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // Call auth service to login user
            var response = await _authService.LoginAsync(dto);

            // Check if login failed
            if (response == null)
                return Unauthorized(new { message = "Invalid email or password" });

            // Return JWT token and user info
            return Ok(response);
        }
    }
}

}
