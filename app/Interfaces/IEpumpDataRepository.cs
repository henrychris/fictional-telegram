using System.Threading.Tasks;
using app.Entities;

namespace app.Interfaces
{
    public interface IEpumpDataRepository
    {
        Task AddUserAsync(EpumpData user);
        Task<bool> CheckUserExistsAsync(string epumpId);
        Task<bool> CheckForChatIdAsync(long chatId);
        Task<EpumpData> GetUserDetailsAsync(long chatId);
        Task<string> GetUserCompanyId(long chatId);
    }
}