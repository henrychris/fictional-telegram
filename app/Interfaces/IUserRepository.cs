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
        Task<string> GetUserState(long chatId);
        Task<AppUser> GetUserByUserNameAsync(string username);
        Task<bool> SaveAllChangesAsync(AppUser user);
        void Update(AppUser user);
    }
}