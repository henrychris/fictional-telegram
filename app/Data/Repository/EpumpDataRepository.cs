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
        private readonly DataContext _context;
        private readonly ErrorHandler _errorHandler;
        public EpumpDataRepository(DataContext context, ErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
            _context = context;
        }

        public async Task<string> GetUserCompanyId(long ChatId)
        {
            using (var context = new DataContext())
            {
                var user = await context.EpumpData.FirstOrDefaultAsync(u => u.ChatId == ChatId);
                if (user == null)
                {
                    return null;
                }
                return user.CompanyId;
            }
        }

        public async Task<EpumpData> GetUserDetailsAsync(long ChatId)
        {
            // add using statement to avoid disposed context error
            try
            {
                using (var context = new DataContext())
                {
                    var userData = await context.EpumpData.AsNoTracking().FirstOrDefaultAsync(x => x.ChatId == ChatId);
                    return userData;
                }
            }
            catch (Exception ex)
            {
                await _errorHandler.HandleErrorAsync(ex);
            }
            return null; // TODO return something else?
        }
    }
}