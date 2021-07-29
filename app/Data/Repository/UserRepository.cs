using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using app.Entities;
using app.Interfaces;
using app.Components;
using System;

namespace app.Data.Repository
{
    public class UserRepository : DbContext, IUserRepository
    {
        private readonly DataContext _context;
        private readonly ErrorHandler _errorHandler;
        public UserRepository(DataContext context, ErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
            _context = context;
        }

        public async Task AddUserAsync(AppUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public Task<bool> CheckUserExistsAsync(long chatId)
        {
            return _context.Users.AnyAsync(u => u.ChatId == chatId);
        }

        public async Task<AppUser> GetUserByIdAndUserNameAsync(long chatId, string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId && u.Username == username);
        }

        public async Task<AppUser> GetUserByIdAsync(long chatId)
        {
            return await _context.Users.FindAsync(chatId);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<string> GetUserStateAsync(long chatId)
        {
            var user = await _context.Users.FindAsync(chatId);
            return user.State;
        }

        public async Task SetUserStateAsync(long chatId, string state)
        {
            try
            {
                /*
                    This  function previously failed because the context was disposed prematurely
                    To prevent this, i am using a new instance of the context.
                */
                using (var context = new DataContext())

                {
                    var user = context.Users.Find(chatId);
                    if (user != null)
                    {
                        user.State = state;
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        await _errorHandler.HandleErrorAsync(new Exception("User not found"));
                    }
                }
            }
            catch (Exception ex)
            {
                await _errorHandler.HandleErrorAsync(ex);
            }
        }

        public async Task<bool> SaveAllChangesAsync(AppUser user)
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}