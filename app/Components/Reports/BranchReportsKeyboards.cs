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

        InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new[]
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

        public BranchReportsKeyboards(ITelegramBotClient botClient, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _botClient = botClient;
        }
        public async Task SendBranchCashflowReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the cashflow report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchCashflowReport");
        }

        public async Task SendProductSummaryReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Product Summary report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchProductSummmaryReport");
        }

        public async Task SendBranchSalesTransactionsReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Sales Transactions report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchSalesTransactionsReport");
        }

        public async Task SendBranchTanksFilledReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Tanks Filled report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchTanksFilledReport");
        }

        public async Task SendBranchVarianceReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            
            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Variance report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchVarianceReport");
        }

        public async Task SendBranchTankReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Tank report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchTankReport");
        }
    }
}