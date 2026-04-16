using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LewisStores.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace LewisStores.Api.Controllers
{
    /// <summary>
    /// Handles authentication flows for client applications.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Request payload for login operations.
        /// </summary>
        public class LoginRequest
        {
            /// <summary>
            /// User email used for sign-in.
            /// </summary>
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// User password. In this mock API this is accepted for testing.
            /// </summary>
            public string Password { get; set; } = string.Empty;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token for authorized calls.
        /// </summary>
        /// <remarks>
        /// In this mock implementation, users are auto-created when the email does not exist.
        /// </remarks>
        /// <param name="request">Login credentials.</param>
        /// <returns>JWT token and user profile details.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // MOCK AUTH: Just accept if it matches any user, or let any non-empty pass for testing.
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                // Auto-create a mock user for testing if it doesn't exist
                user = new Models.User { Email = request.Email, Password = "password", Role = "Customer" };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Create Mock Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("ThisIsAMockSuperSecretKeyForLewisStoresApisThatIsAtLeast32Bytes");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "LewisStoresMockIssuer",
                Audience = "LewisStoresMockAudience"
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                Token = tokenHandler.WriteToken(token),
                User = new { user.Id, user.Email, user.Role }
            });
        }
    }
}
