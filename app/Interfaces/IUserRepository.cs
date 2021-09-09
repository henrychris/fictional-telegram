using System.Threading.Tasks;
using app.Entities;

namespace app.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(AppUser user);
        Task<bool> CheckUserExistsAsync(long chatId);
        Task<bool> CheckForEpumpIdAsync(string id);
        Task DeleteUser(long chatId);
        Task<AppUser> GetUserByIdAndUserNameAsync(long chatId, string username);
        Task<AppUser> GetUserByIdAsync(long chatId);
        Task<string> GetUserStateAsync(long chatId);
        Task<string> GetCurrentBranchIdAsync(long chatId);
        Task<AppUser> GetUserByUserNameAsync(string username);
        Task<bool> SaveAllChangesAsync(AppUser user);
        Task SetUserStateAsync(long chatId, string state);
        Task SetCurrentBranchIdAsync(long chatId, string branchId);
        Task SetCurrentBranchIdToNull(long chatId);
        void Update(AppUser user);
        Task FindAndUpdateUserWithEpumpDataAsync(long chatId, string epumpId);

        Task SetUserEmail(long chatId, string email);
        Task<string> GetUserEmail(long chatId);
    }
}