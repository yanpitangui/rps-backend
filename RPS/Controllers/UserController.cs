using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPS.Models;
using RPS.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RPS.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        [HttpGet("info")]
        public async Task<IActionResult> getInfo()
        {
            var userEmail = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
            return Ok(new ApiResponse<User>(true, null, await _userService.FindByEmail(userEmail)));
        }
    }
}