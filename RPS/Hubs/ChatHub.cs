using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RPS.Services;
using System;
using System.Threading.Tasks;

namespace RPS.Hubs
{
    //[Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService chatService;
        public ChatHub(IChatService chatService)
        {
            this.chatService = chatService;
        }
        public async Task SendMessage(string message)
        {
            var sender = Context.UserIdentifier;
            await chatService.SaveMessage(sender, message);
            await Clients.Others.SendAsync("ReceiveMessage", message, DateTime.Now, sender);
        }
    }
}
