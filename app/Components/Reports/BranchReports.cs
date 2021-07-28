using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace app.Components.Reports
{
    public class BranchReports
    {
        private readonly ITelegramBotClient _botClient;
        public BranchReports(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task<Message> SendBranchCashflowReportAsync(CallbackQuery query, Message message)
        {
            return await _botClient.SendTextMessageAsync(message.Chat.Id, "Testing");
        }
    }
}