using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPS.Models;
using RPS.Services;

namespace RPS.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly IChatService chatService;

        public ChatController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpGet("latest-messages")]
        [ProducesResponseType(typeof(ApiResponse<List<ChatMessage>>), 200)]
        public async Task<IActionResult> GetLatestMessages()
        {
            return Ok(new ApiResponse<List<ChatMessage>>(true, null, await chatService.GetChatMessages()));
        }

    }
}