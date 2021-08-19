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
            // TODO check if the DI works properly
            _errorHandler = errorHandler;
            _context = context;
        }

        // ! where AsNoTracking is Enabled, values can only be read.
        // ! and db would not be updated.

        public async Task AddUserAsync(AppUser user)
        {
            using (var context = new DataContext())
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckUserExistsAsync(long chatId)
        {
            // ! Investigate the reason for the invalid operation exception.
            using (var context = new DataContext())
                try
                {
                    return await context.Users.AsNoTracking().AnyAsync(u => u.ChatId == chatId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            return false;
        }

        public async Task<AppUser> GetUserByIdAndUserNameAsync(long chatId, string username)
        {
            using (var context = new DataContext())
                return await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.ChatId == chatId && u.Username == username);
        }

        public async Task<AppUser> GetUserByIdAsync(long chatId)
        {
            using (var context = new DataContext())
                return await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.ChatId == chatId);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            using (var context = new DataContext())
                return await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<string> GetUserStateAsync(long chatId)
        {
            using (var context = new DataContext())
            {
                var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.ChatId == chatId);
                return user.State;
            }
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
            using (var context = new DataContext())
                return await context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            using (var context = new DataContext())
                context.Entry(user).State = EntityState.Modified;
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
    }
}