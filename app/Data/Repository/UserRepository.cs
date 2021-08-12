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

        // ! where AsNoTracking is Enabled, values can only be read.
        // ! and db would not be updated.

        public async Task AddUserAsync(AppUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public Task<bool> CheckUserExistsAsync(long chatId)
        {
            _context.Users.AsNoTracking();
            return _context.Users.AnyAsync(u => u.ChatId == chatId);
        }

        public async Task<AppUser> GetUserByIdAndUserNameAsync(long chatId, string username)
        {
            _context.Users.AsNoTracking();
            return await _context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId && u.Username == username);
        }

        public async Task<AppUser> GetUserByIdAsync(long chatId)
        {
            _context.Users.AsNoTracking();
            return await _context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            _context.Users.AsNoTracking();
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<string> GetUserStateAsync(long chatId)
        {
            _context.Users.AsNoTracking();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.ChatId == chatId);
            return user.State;
        }

        public async Task SetUserStateAsync(long chatId, string state)
        {
            try
            {
                /*
                    This  function previously failed because the context was disposed prematurely
                    To prevent this, i am using a new instance of the context.
                    It will be disposed automatically.
                */
                using (var context = new DataContext())
                {
                    var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
                    if (user != null)
                    {
                        // user.State.Remove();
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

        public async Task SetCurrentBranchIdAsync(long chatId, string branchId)
        {
            using (var context = new DataContext())
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
                if (user != null)
                {
                    user.CurrentBranch = branchId;
                    await context.SaveChangesAsync();
                }
                else
                {
                    await _errorHandler.HandleErrorAsync(new Exception("User not found"));
                }
            }
        }

        public async Task SetCurrentBranchIdToNull(long chatId)
        {
            await using var context = new DataContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
            if (user != null)
            {
                user.CurrentBranch = null;
                await context.SaveChangesAsync();
            }
        }

        public async Task<string> GetCurrentBranchIdAsync(long chatId)
        {
            AppUser user = null;
            await using var context = new DataContext();
            try
            {
                user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.ChatId == chatId);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

            // Whenever there is a null value, it shall return null
            return user?.CurrentBranch;
        }
    }
}