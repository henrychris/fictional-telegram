using System;
using System.Threading.Tasks;
using app.Entities;

namespace app.Interfaces
{
    public interface ILoginStatusRepository
    {
        Task AddAsync(LoginStatusTelegram user);
        Task AddAsync(LoginStatusEpump user);
        Task<bool> IsUserLoggedInAsync(long chatId, string epumpId);
        Task<bool> IsUserLoggedInAsync_Epump(string epumpId);
        Task<bool> IsUserLoggedInAsync_Telegram(long chatId);
    }
}