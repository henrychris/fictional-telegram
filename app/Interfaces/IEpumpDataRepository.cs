using System.Threading.Tasks;
using app.Entities;

namespace app.Interfaces
{
    public interface IEpumpDataRepository
    {
        Task AddUserAsync(EpumpData user);
        Task<bool> CheckForChatIdAsync(long chatId);
        Task<bool> CheckUserExistsAsync(string Id);
        Task DeleteUserAsync(string epumpId);
        Task<EpumpData> GetUserDetailsAsync(long chatId);
    }
}