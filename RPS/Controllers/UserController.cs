using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPS.Context;
using RPS.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RPS.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        [ProducesResponseType(typeof(ApiResponse<User>), 200)]
        [HttpGet("getInfo")]
        public async Task<IActionResult> getInfo()
        {
            var userEmail = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
            return Ok(new ApiResponse<User>(true, null, await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail)));
        }
    }
}