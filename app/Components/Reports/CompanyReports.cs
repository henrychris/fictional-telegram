using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace app.Components.Reports
{
    public class CompanyReports
    {
        private readonly ITelegramBotClient _botClient;
        public CompanyReports(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        // TODO
        /*
            receive callback data and run it through a switch to get the time frame for report.
            ! figure out how to store user company Ids.
            ? send typing action while pdf processes
        */

        public async Task<Message> SendCompanyBranchSalesReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company BranchSales: {query.Data}");
        }

        public async Task<Message> SendCompanyCashflowReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company Cashflow: {query.Data}");
        }

        public async Task<Message> SendCompanySalesSummaryReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company SalesSummary: {query.Data}");
        }

        public async Task<Message> SendCompanyTankStockReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company TankStock: {query.Data}");
        }

        public async Task<Message> SendCompanyTanksFilledReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company TanksFilled: {query.Data}");
        }

        public async Task<Message> SendCompanyVarianceReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company Variance: {query.Data}");
        }

        public async Task<Message> SendCompanyExpenseCategoriesReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company ExpenseCategories: {query.Data}");
        }

        public async Task<Message> SendCompanyZonesReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company Zones: {query.Data}");
        }

        public async Task<Message> SendCompanyOutstandingPaymentsReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company OutstandingPayments: {query.Data}");
        }

        public async Task<Message> SendCompanyWalletReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company Wallet: {query.Data}");
        }
        
        public async Task<Message> SendCompanyRetainershipReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company Retainership: {query.Data}");
        }

        public async Task<Message> SendCompanyWalletFundRequestReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company Wallet Fund Request: {query.Data}");
        }
    }
}