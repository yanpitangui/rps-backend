using Microsoft.EntityFrameworkCore;
using RPS.Context;
using RPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPS
{
    public interface IAuthService
    {
        Task<User> Authenticate(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload);

        Task<List<User>> getAll();
    }

    public class AuthService : IAuthService
    {
        ApplicationDbContext _dbContext;
        public AuthService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User> Authenticate(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload)
        {
            return await this.FindUserOrAdd(payload);
        }

        private async Task<User> FindUserOrAdd(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload)
        {
            var u = await _dbContext.Users.Where(x => x.Email == payload.Email).FirstOrDefaultAsync();

            if (u == null)
            {
                u = new User()
                {
                    Id = Guid.NewGuid(),
                    Nickname = payload.Name,
                    Email = payload.Email
                };
                _dbContext.Users.Add(u);
                await _dbContext.SaveChangesAsync();
            }
            return u;
        }

        public async Task<List<User>> getAll()
        {
            return await _dbContext.Users.AsNoTracking().ToListAsync();
        }
    }
}
