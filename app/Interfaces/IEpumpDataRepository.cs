using System.Threading.Tasks;
using app.Entities;

namespace app.Interfaces
{
    public interface IEpumpDataRepository
    {
        Task<EpumpData> GetUserDetailsAsync(long ChatId);
        Task<string> GetUserCompanyId(long ChatId);
    }
}