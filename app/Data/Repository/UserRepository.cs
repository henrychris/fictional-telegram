using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using app.Components;
using app.Entities;
using app.Interfaces;

namespace app.Data.Repository
{
    public class UserRepository : DbContext, IUserRepository
    {
        private readonly ErrorHandler _errorHandler;
        public UserRepository(ErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        public async Task AddUserAsync(AppUser user)
        {
            await using var context = new DataContext();
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckUserExistsAsync(long chatId)
        {
            // ! Investigate the reason for the invalid operation exception.
            await using var context = new DataContext();
            return await context.Users.AsNoTracking().AnyAsync(u => u.ChatId == chatId);
        }

        public async Task<bool> CheckForEpumpIdAsync(string id)
        {
            await using var context = new DataContext();
            return await context.Users.AsNoTracking().AnyAsync(u => u.EpumpDataId == id);
        }

        public async Task FindAndUpdateUserWithEpumpDataAsync(long chatId, string epumpId)
        {
            await using var context = new DataContext();

            var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
            if (user != null)
            {
                user.EpumpDataId = epumpId;
                await context.SaveChangesAsync();
            }
        }

        public async Task<string> GetCurrentBranchIdAsync(long chatId)
        {
            await using var context = new DataContext();

            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.ChatId == chatId);
            return user?.CurrentBranch;
        }

        public async Task<AppUser> GetUserByIdAndUserNameAsync(long chatId, string username)
        {
            await using var context = new DataContext();
            return await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.ChatId == chatId && u.Username == username);
        }

        public async Task<AppUser> GetUserByIdAsync(long chatId)
        {
            await using var context = new DataContext();
            return await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.ChatId == chatId);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            await using var context = new DataContext();
            return await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<string> GetUserStateAsync(long chatId)
        {
            await using var context = new DataContext();
            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.ChatId == chatId);
            return user.State;
        }

        public async Task<bool> SaveAllChangesAsync(AppUser user)
        {
            await using var context = new DataContext();
            return await context.SaveChangesAsync() > 0;
        }

        public async Task SetCurrentBranchIdAsync(long chatId, string branchId)
        {
            await using var context = new DataContext();
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

        public async Task SetUserStateAsync(long chatId, string state)
        {
            /*
                This  function previously failed because the context was disposed prematurely
                To prevent this, i am using a new instance of the context.
                It will be disposed automatically.
            */
            await using var context = new DataContext();

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

        public void Update(AppUser user)
        {
            using var context = new DataContext();
            context.Entry(user).State = EntityState.Modified;
        }

        public async Task DeleteUser(long chatId)
        {
            await using var context = new DataContext();
            var user = new AppUser { ChatId = chatId };
            context.Users.Attach(user);
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        public async Task SetUserEmail(long chatId, string email)
        {
            await using var context = new DataContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
            user.Email = email;
            await context.SaveChangesAsync();
        }

        public async Task<string> GetUserEmail(long chatId)
        {
            await using var context = new DataContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
            var email = user.Email;
            return email;
        }
    }
}