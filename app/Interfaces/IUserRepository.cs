using System;
using System.Threading.Tasks;
using app.Entities;

namespace app.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(AppUser user);
        Task<bool> CheckUserExistsAsync(long chatId);
        Task<AppUser> GetUserByIdAndUserNameAsync(long chatId, string username);
        Task<AppUser> GetUserByIdAsync(long chatId);
        Task<string> GetUserStateAsync(long chatId);
        Task<AppUser> GetUserByUserNameAsync(string username);
        Task<bool> SaveAllChangesAsync(AppUser user);
        Task SetUserStateAsync(long chatId, string state);
        void Update(AppUser user);
    }
}