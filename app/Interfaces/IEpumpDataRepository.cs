using System.Threading.Tasks;
using app.Entities;

namespace app.Interfaces
{
    public interface IEpumpDataRepository
    {
        Task<EpumpData> GetUserDetailsAsync(long chatId);
        Task<string> GetUserCompanyId(long chatId);
    }
}