using Microsoft.EntityFrameworkCore;
using RPS.Context;
using RPS.Generators;
using RPS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RPS.Services
{
    public interface IUserService
    {
        Task<User> FindUserOrAdd(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload);
        Task<string> GenerateUserName();
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly NicknameGenerator _generator;
        public UserService(ApplicationDbContext dbContext, NicknameGenerator generator)
        {
            _dbContext = dbContext;
            _generator = generator;
        }

        public async Task<User> FindUserOrAdd(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload)
        {
            var u = await _dbContext.Users.Where(x => x.Email == payload.Email).FirstOrDefaultAsync();

            if (u == null)
            {
                u = new User()
                {
                    Id = Guid.NewGuid(),
                    Nickname = await GenerateUserName(),
                    Email = payload.Email
                };
                _dbContext.Users.Add(u);
                await _dbContext.SaveChangesAsync();
            }
            return u;
        }

        public async Task<string> GenerateUserName()
        {

            string uname = string.Empty;
            bool exists = true;
            int repeat = 0;
            while (exists && repeat < 10)
            {
                uname = _generator.Generate();
                exists = await _dbContext.Users.AnyAsync(u => u.Nickname == uname);
                repeat++;
            }
            return uname;
        }
    }
}
