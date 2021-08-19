using System.Threading.Tasks;
using app.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace app.Components.Reports
{
    public class CompanyReportsKeyboards
    {
        private InlineKeyboardMarkup _keyboard = new(new[]
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

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Sales report", replyMarkup: _keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyBranchSalesReport");
        }

        public async Task SendCompanyCashflowReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Cashflow report", replyMarkup: _keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyCashflowReport");
        }

        public async Task SendCompanySalesSummaryReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Sales Summary report", replyMarkup: _keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanySalesSummaryReport");
        }

        public async Task SendCompanyTankStockReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Tank Stock report", replyMarkup: _keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyTankStockReport");
        }

        public async Task<Message> SendCompanyTanksFilledReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyTanksFilledReport");

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Input time in this format: 'Jan 01, 2000'", replyMarkup: new ForceReplyMarkup());
            return null;
        }

        public async Task SendCompanyVarianceReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Variance report", replyMarkup: _keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyVarianceReport");
        }

        public async Task SendCompanyExpenseCategoriesReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Expense Categories report", replyMarkup: _keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyExpenseCategoriesReport");
        }

        public async Task SendCompanyZonesReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            // await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Zones report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyZonesReport");
        }

        public async Task SendCompanyOutstandingPaymentsReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Outstanding Payments report", replyMarkup: _keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyOutstandingPaymentsReport");
        }

        public async Task SendCompanyWalletReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Wallet report", replyMarkup: _keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyWalletReport");
        }

        public async Task SendCompanyRetainershipReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Retainership report", replyMarkup: _keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyRetainershipReport");
        }

        public async Task SendCompanyPOSTransactionsReportKeyboard(Message message)
        {
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the POS-Transactions report", replyMarkup: _keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyPOSTransactionsReport");
        }

        public async Task SendCompanyWalletFundRequestReportKeyboard(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            _keyboard = new InlineKeyboardMarkup(new[]
            {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Approved", "approved"),
                InlineKeyboardButton.WithCallbackData("Pending", "pending")
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData("Show All", "show all")
            }});

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Status?", replyMarkup: _keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "CompanyWalletFundRequestReport");
        }
    }
}