using System;
using System.Threading.Tasks;
using app.Entities;
using app.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Data.Repository
{
    public class LoginStatusMethods : ILoginStatusRepository
    {
        public async Task AddAsync(LoginStatusTelegram user)
        {
            await using var context = new DataContext();
            await context.loginStatusTelegram.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task AddAsync(LoginStatusEpump user)
        {
            await using var context = new DataContext();
            await context.loginStatusEpump.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<bool> IsUserLoggedInAsync(long chatId, string epumpId)
        {
            await using var context = new DataContext();
            var telegramUser = await context.loginStatusTelegram.AsNoTracking().AnyAsync(u => u.UserChatId == chatId);
            var epumpUser = await context.loginStatusEpump.AsNoTracking().AnyAsync(u => u.EpumpDataId == epumpId);

            return telegramUser && epumpUser;
        }

        public async Task<bool> IsUserLoggedInAsync_Epump(string id)
        {
            await using var context = new DataContext();
            return await context.loginStatusEpump.AnyAsync(u => u.EpumpDataId == id);
        }

        public async Task<bool> IsUserLoggedInAsync_Telegram(long chatId)
        {
            await using var context = new DataContext();
            return await context.loginStatusTelegram.AnyAsync(u => u.UserChatId == chatId);
        }
    }
}