using Microsoft.EntityFrameworkCore;
using RPS.Context;
using RPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPS.Services
{
    public interface IChatService
    {
        Task SaveMessage(string Sender, string Message);
        Task<List<ChatMessage>> GetChatMessages();
    }

    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _dbContext;

        public ChatService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task SaveMessage(string Sender, string Message)
        {
            _dbContext.Add(new ChatMessage { Sender = Sender, Message = Message, SendDate = DateTime.Now });
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ChatMessage>> GetChatMessages()
        {
            return await _dbContext.Messages.OrderByDescending(x=> x.Id).Take(200).OrderBy(x=>x.Id).ToListAsync();
        }
    }
}
