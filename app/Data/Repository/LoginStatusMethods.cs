using System;
using System.Threading.Tasks;
using app.Entities;
using app.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Data.Repository
{
    public class LoginStatusMethods : ILoginStatusRepository
    {
        private readonly DataContext _context;

        public LoginStatusMethods(DataContext context)
        {
            _context = context;
        }

        public async Task AddAsync(LoginStatusTelegram user)
        {
            await _context.loginStatusTelegram.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(LoginStatusEpump user)
        {
            await _context.loginStatusEpump.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserLoggedInAsync(long chatId, string epumpId)
        {
            var telegramUser = await _context.loginStatusTelegram.AsNoTracking().AnyAsync(u => u.UserChatId == chatId);
            var epumpUser = await _context.loginStatusEpump.AsNoTracking().AnyAsync(u => u.EpumpDataId == epumpId);

            return telegramUser && epumpUser;
        }

        public async Task<bool> IsUserLoggedInAsync_Epump(string id)
        {
            return await _context.loginStatusEpump.AnyAsync(u => u.EpumpDataId == id);
        }

        public async Task<bool> IsUserLoggedInAsync_Telegram(long chatId)
        {
            return await _context.loginStatusTelegram.AnyAsync(u => u.UserChatId == chatId);
        }

        public async Task SetEpumpLoginStatusAsync(string epumpId, DateTime dateTime, bool status)
        {
            var epumpUser = await _context.loginStatusEpump.FirstOrDefaultAsync(u => u.EpumpDataId == epumpId);
            epumpUser.IsLoggedIn = status;
            await _context.SaveChangesAsync();
        }

        public async Task SetTelegramLoginStatusAsync(long chatId, DateTime dateTime, bool status)
        {
            var telegramUser = await _context.loginStatusTelegram.FirstOrDefaultAsync(u => u.UserChatId == chatId);
            telegramUser.IsLoggedIn = status;
            await _context.SaveChangesAsync();
        }
    }
}