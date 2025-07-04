// Controllers/DebugController.cs - Temporary for testing
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GardenCentresApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "API is working", timestamp = DateTime.Now });
        }

        [HttpGet("auth-test")]
        [Authorize]
        public IActionResult AuthTest()
        {
            var region = User.FindFirst("Region")?.Value;
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            return Ok(new
            {
                region = region,
                allClaims = claims,
                isAuthenticated = User.Identity?.IsAuthenticated ?? false
            });
        }

        [HttpGet("headers")]
        public IActionResult Headers()
        {
            var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            return Ok(new { headers = headers });
        }

        [HttpGet("auth-headers")]
        [Authorize]
        public IActionResult AuthHeaders()
        {
            var authHeader = Request.Headers.Authorization.ToString();
            var hasBearer = authHeader.StartsWith("Bearer ");

            return Ok(new
            {
                authorizationHeader = authHeader,
                hasBearer = hasBearer,
                headerLength = authHeader.Length
            });
        }

        // Test endpoint without [Authorize] but checking authentication manually
        [HttpGet("manual-auth-check")]
        public IActionResult ManualAuthCheck()
        {
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            var authType = User.Identity?.AuthenticationType;
            var name = User.Identity?.Name;

            return Ok(new
            {
                isAuthenticated = isAuthenticated,
                authenticationType = authType,
                name = name,
                claimsCount = User.Claims.Count()
            });
        }
    }
}