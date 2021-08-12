using System;
using System.Threading.Tasks;
using app.Components;
using app.Entities;
using app.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Data.Repository
{
    public class EpumpDataRepository : IEpumpDataRepository
    {
        private readonly ErrorHandler _errorHandler;
        public EpumpDataRepository(ErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        public async Task<string> GetUserCompanyId(long chatId)
        {
            await using var context = new DataContext();
            var user = await context.EpumpData.FirstOrDefaultAsync(u => u.ChatId == chatId);
            return user == null ? null : user.CompanyId;
        }

        public async Task<EpumpData> GetUserDetailsAsync(long chatId)
        {
            // add using statement to avoid disposed context error
            try
            {
                await using var context = new DataContext();
                var userData = await context.EpumpData.AsNoTracking().FirstOrDefaultAsync(x => x.ChatId == chatId);
                return userData;
            }
            catch (Exception ex)
            {
                await _errorHandler.HandleErrorAsync(ex);
            }
            return null; // TODO return something else?
        }
    }
}