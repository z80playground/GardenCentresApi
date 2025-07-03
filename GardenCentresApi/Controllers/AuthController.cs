using GardenCentresApi.Data;
using GardenCentresApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GardenCentresApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly GardenCentreContext _context;

        public AuthController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            GardenCentreContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model.Region != "US" && model.Region != "UK")
            {
                return BadRequest("Region must be 'US' or 'UK'");
            }

            var user = new IdentityUser { UserName = model.Username, Email = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Create user profile with region
            var profile = new UserProfile { UserId = user.Id, Region = model.Region };
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Get region from UserProfile
                var profile = await _context.UserProfiles
                    .Where(up => up.UserId == user.Id)
                    .Select(up => up.Region)
                    .FirstOrDefaultAsync();

                if (profile == null)
                {
                    return BadRequest("User profile not found");
                }

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim("Region", profile)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds);

                return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            return Unauthorized();
        }
    }

    public class LoginModel
    {
        required public string Username { get; set; }
        required public string Password { get; set; }
    }

    public class RegisterModel
    {
        required public string Username { get; set; }
        required public string Password { get; set; }
        required public string Region { get; set; } // US or UK
    }
}