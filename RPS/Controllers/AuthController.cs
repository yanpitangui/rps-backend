using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Logging;
using RPS.Models;
using System.Diagnostics;
using RPS.Helpers;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace RPS.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private JWTConfig _config;

        public AuthController(ILogger<AuthController> logger, IAuthService authService, IOptions<JWTConfig> config)
        {
            _logger = logger;
            this._authService = authService;
            _config = config.Value;
        }

        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<JWTToken>), 200)]
        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody]UserView userView)
        {
            try
            {
                //SimpleLogger.Log("userView = " + userView.tokenId);
                var payload = await GoogleJsonWebSignature.ValidateAsync(userView.tokenId, new GoogleJsonWebSignature.ValidationSettings() { Audience = new[] { _config.GoogleClientId } });
                if (!payload.EmailVerified) return StatusCode(403, new ApiResponse(false, "Forbidden"));
                var user = await _authService.Authenticate(payload);
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, Security.Encrypt(_config.JwtEmailEncryption,user.Email)),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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

        [HttpGet("test")]
        public async Task<List<User>> getAll()
        {
            return await _authService.getAll();
        }
    }
}
