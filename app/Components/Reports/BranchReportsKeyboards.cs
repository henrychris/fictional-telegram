using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace app.Components.Reports
{
    public class BranchReportsKeyboards
    {
        private readonly ITelegramBotClient _botClient;
        public BranchReportsKeyboards(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }
        public async Task<Message> SendCashflowReport(Message message)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Today", "Today"),
                InlineKeyboardButton.WithCallbackData("Yesterday", "Yesterday")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Last 7 Days", "Week"),
                InlineKeyboardButton.WithCallbackData("Last 30 Days", "Month")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Back", "BranchReports")
            }
            });

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the cashflow report", replyMarkup: new ForceReplyMarkup());
            return  await _botClient.SendTextMessageAsync(message.Chat.Id, message.ReplyToMessage.Text);
        }

        // public void SetReportRangeKeyboard(Message message)
        // {
        //     var keyboard = new InlineKeyboardMarkup(new[]
        //     {
        //     new[]
        //     {
        //         InlineKeyboardButton.WithCallbackData("Today"),
        //         InlineKeyboardButton.WithCallbackData("Yesterday")
        //     },
        //     new[]
        //     {
        //         InlineKeyboardButton.WithCallbackData("Last 7 Days", "Week"),
        //         InlineKeyboardButton.WithCallbackData("Last 30 Days", "Month")
        //     },
        //     new[]
        //     {
        //         InlineKeyboardButton.WithCallbackData("Back", "Reports")
        //     }
        //     });
        // }
    }
}