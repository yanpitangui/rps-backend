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
        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody]UserView userView)
        {
            try
            {
                //SimpleLogger.Log("userView = " + userView.tokenId);
                var payload = await GoogleJsonWebSignature.ValidateAsync(userView.tokenId, new GoogleJsonWebSignature.ValidationSettings() { Audience = new[] { _config.GoogleClientId } });
                var user = await _authService.Authenticate(payload);
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, Security.Encrypt(_config.JwtEmailEncryption,user.email)),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.JwtSecret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken("RPS_API",
                  _config.GoogleClientId,
                  claims,
                  expires: DateTime.Now.AddMinutes(60),
                  signingCredentials: creds);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }
            return BadRequest();
        }

        [HttpGet("test")]
        public async Task<List<User>> getAll()
        {
            return await _authService.getAll();
        }
    }
}
