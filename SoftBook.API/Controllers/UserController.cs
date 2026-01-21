using Microsoft.AspNetCore.Mvc;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.Entities;
using SoftBook.Application.Helpers; 
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SoftBook.Infrastructure.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;
        private readonly IConfiguration _configuration;
        private readonly ITenantProvider _tenantProvider;

        public UserController(
            IUserService userService, 
            ICompanyService companyService, 
            IConfiguration configuration,
            ITenantProvider tenantProvider)
        {
            _userService = userService;
            _companyService = companyService;
            _configuration = configuration;
            _tenantProvider = tenantProvider;
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateUser([FromBody] LoginRequest request)
        {
            try
            {
                // 1. Get Company Connection String
                Console.WriteLine($"[Login Diagnostic] Attempting login for CompanyId: {request.CompanyId}, User: {request.EmailUsername}");
                var company = await _companyService.GetCompanyByIdAsync(request.CompanyId);
                if (company == null || string.IsNullOrEmpty(company.ConnectionString))
                {
                    Console.WriteLine($"[Login Diagnostic] Failed: Invalid CompanyId {request.CompanyId}");
                    return BadRequest("Invalid Company ID");
                }

                Console.WriteLine($"[Login Diagnostic] Resolved ConnectionString: {company.ConnectionString}");

                // 2. Set Connection String for Multi-tenancy
                _tenantProvider.ConnectionString = company.ConnectionString;

                // 3. Validate User
                var passwordHash = PasswordHelper.HashPassword(request.Password);
                Console.WriteLine($"[Login Diagnostic] Generated Hash: {passwordHash}");
                
                var loginResponse = await _userService.ValidateUserAsync(request.EmailUsername, passwordHash, request.CompanyId);

                // Fallback for legacy users (plain text password)
                if (loginResponse == null)
                {
                    Console.WriteLine("[Login Diagnostic] Hashed validation failed. Trying plain text fallback...");
                    loginResponse = await _userService.ValidateUserAsync(request.EmailUsername, request.Password, request.CompanyId);
                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.User.Email))
                    {
                        Console.WriteLine("[Login Diagnostic] Plain text fallback succeeded! Auto-migrating to hash...");
                        await _userService.UpdatePasswordAsync(loginResponse.User.Email, passwordHash);
                    }
                }

                if (loginResponse == null)
                {
                   Console.WriteLine("[Login Diagnostic] All validation attempts failed.");
                   return Unauthorized("Invalid email/username or password. Check API console for details.");
                }

                // 4. Generate JWT Token
                loginResponse.Token = GenerateJwtToken(loginResponse.User, request.CompanyId);
                Console.WriteLine("[Login Diagnostic] Login Successful.");

                // Return token and user info
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                // Log exception here
                Console.WriteLine($"Login Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                 var passwordHash = PasswordHelper.HashPassword(request.Password);
                 var newUser = await _userService.InsertUserAsync(
                    request.Username,
                    passwordHash,
                    request.Email,
                    request.FullName,
                    request.Phone);
                
                if (newUser != null) return Ok(newUser);
                return BadRequest("Failed to register user.");
            }
            catch (Exception ex)
            {
                // Return original exception message to handle "Username exists" etc.
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            try 
            {
                var success = await _userService.UpdatePasswordAsync(request.Email, request.NewPasswordHash);
                if (success) return Ok(true);
                return BadRequest("Failed to update password.");
            }
            catch (Exception ex)
            {
                 return BadRequest(ex.Message);
            }
        }
        [HttpGet("{userId}/permissions")]
        public async Task<IActionResult> GetPermissions(int userId)
        {
            var permissions = await _userService.GetSidebarPermissionsAsync(userId);
            return Ok(permissions);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            if (id != user.Id) return BadRequest();
            await _userService.UpdateAsync(user);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return Ok();
        }

        private string GenerateJwtToken(User user, int tenantId)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not found"));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("TenantId", tenantId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"] ?? "60")),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public int CompanyId { get; set; }
        public string EmailUsername { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
    }

    public class UpdatePasswordRequest
    {
        public string Email { get; set; }
        public string NewPasswordHash { get; set; }
    }
}
