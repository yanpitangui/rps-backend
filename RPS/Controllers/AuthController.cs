
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RPS.Helpers;
using RPS.Models;
using RPS.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RPS.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly JWTConfig _config;

        public AuthController(ILogger<AuthController> logger, IAuthService authService, IOptions<JWTConfig> config)
        {
            _logger = logger;
            _authService = authService;
            _config = config.Value;
        }

        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<JWTToken>), 200)]
        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody]UserView userView)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(userView.tokenId, new GoogleJsonWebSignature.ValidationSettings() { Audience = new[] { _config.GoogleClientId } });
                if (!payload.EmailVerified) return StatusCode(403, new ApiResponse(false, "Forbidden"));
                var user = await _authService.Authenticate(payload);
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.NameId, user?.Id?.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, payload.Email),
                    new Claim(JwtRegisteredClaimNames.GivenName, user.Nickname)
                };

                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.JwtSecret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken("RPS_API",
                  _config.GoogleClientId,
                  claims,
                  expires: DateTime.Now.AddMinutes(60),
                  signingCredentials: creds);
                return Ok(new ApiResponse<JWTToken>(true, string.Empty, new JWTToken
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpDate = token.ValidTo
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(false, ex.Message));
            }
        }
    }
}
