using System.Threading.Tasks;
using app.Entities;
using app.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Data.Repository
{
    public class EpumpDataRepository : IEpumpDataRepository
    {
        public async Task AddUserAsync(EpumpData user)
        {
            await using var context = new DataContext();
            await context.EpumpData.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckUserExistsAsync(string epumpId)
        {
            await using var context = new DataContext();
            return await context.EpumpData.AsNoTracking().AnyAsync(e => e.ID == epumpId);
        }

        public async Task<bool> CheckForChatIdAsync(long chatId)
        {
            await using var context = new DataContext();
            return await context.EpumpData.AsNoTracking().AnyAsync(e => e.ChatId == chatId);
        }

        public async Task<string> GetUserCompanyId(long chatId)
        {
            await using var context = new DataContext();
            var user = await context.EpumpData.AsNoTracking().FirstOrDefaultAsync(u => u.ChatId == chatId);
            return user?.CompanyId;
        }

        public async Task<EpumpData> GetUserDetailsAsync(long chatId)
        {
            await using var context = new DataContext();
            var userData = await context.EpumpData.AsNoTracking().FirstOrDefaultAsync(x => x.ChatId == chatId);
            return userData;
        }

        public async Task DeleteUserAsync(string epumpId)
        {
            await using var context = new DataContext();
            var user = new EpumpData { ID = epumpId };
            context.EpumpData.Attach(user);
            context.EpumpData.Remove(user);
            await context.SaveChangesAsync();
        }
    }
}