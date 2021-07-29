using System.Threading.Tasks;
using app.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace app.Components.Reports
{
    public class CompanyReportsKeyboards
    {
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
                InlineKeyboardButton.WithCallbackData("Back", "CompanyReports")
            }
            });

        private readonly ITelegramBotClient _botClient;
        private readonly IUserRepository _userRepository;
        public CompanyReportsKeyboards(ITelegramBotClient botClient, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _botClient = botClient;
        }

        public async Task SendCompanyBranchSalesReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Sales report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyBranchSalesReport");
        }

        public async Task SendCompanyCashflowReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Cashflow report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyCashflowReport");
        }

        public async Task SendCompanySalesSummaryReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Sales Summary report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanySalesSummaryReport");
        }

        public async Task SendCompanyTankStockReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Tank Stock report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyTankStockReport");
        }

        public async Task SendCompanyTanksFilledReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Tanks Filled report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyTanksFilledReport");
        }

        public async Task SendCompanyVarianceReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Variance report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyVarianceReport");
        }

        public async Task SendCompanyExpenseCategoriesReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Expense Categories report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyExpenseCategoriesReport");
        }

        public async Task SendCompanyZonesReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Zones report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyZonesReport");
        }

        public async Task SendCompanyOutstandingPaymentsReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Outstanding Payments report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyOutstandingPaymentsReport");
        }

        public async Task SendCompanyWalletReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Wallet report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyWalletReport");
        }

        public async Task SendCompanyRetainershipReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            
            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Retainership report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyRetainershipReport");
        }

        public async Task SendCompanyWalletFundRequestReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyWalletFundRequestReport");
        }
    }
}