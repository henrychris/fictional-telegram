using System.Threading.Tasks;
using app.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace app.Components.Reports
{
    public class BranchReportsKeyboards
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUserRepository _userRepository;
        public BranchReportsKeyboards(ITelegramBotClient botClient, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _botClient = botClient;
        }
        public async Task SendBranchCashflowReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

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

            await _userRepository.SetUserStateAsync(message.From.Id, "BranchCashflowReport");
            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the cashflow report", replyMarkup: keyboard);
            // return await _botClient.SendTextMessageAsync(message.Chat.Id, message.ReplyToMessage.Text);
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