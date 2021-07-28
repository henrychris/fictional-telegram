using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using app.Entities;
using app.Interfaces;

namespace app.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
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
            return await _context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId).ContinueWith(t => t.Result.State);
        }

        public async Task SetUserStateAsync(long chatId, string state)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);
            if (user != null)
            {
                user.State = state;
                await _context.SaveChangesAsync();
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