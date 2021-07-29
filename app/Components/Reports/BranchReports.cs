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

        // TODO
        /*
            receive callback data and run it through a switch to get the time frame for report.
            ! figure out how to store user company Ids.
            ? send typing action while pdf processes
        */

        public async Task<Message> SendBranchCashflowReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, query.Message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Branch Cashflow: {query.Data}");
        }

        public async Task<Message> SendProductSummaryReportAsync(CallbackQuery query, Message message)
        {
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, query.Message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Product Summary: {query.Data}");
        }

        public async Task<Message> SendBranchSalesTransactionsReportAsync(CallbackQuery query, Message message)
        {
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, query.Message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Branch Sales Transactions: {query.Data}");
        }

        public async Task<Message> SendBranchTanksFilledReportAsync(CallbackQuery query, Message message)
        {
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, query.Message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Branch Tanks Filled: {query.Data}");
        }

        public async Task<Message> SendBranchVarianceReportAsync(CallbackQuery query, Message message)
        {
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, query.Message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Branch Variance: {query.Data}");
        }

        public async Task<Message> SendBranchTankReportAsync(CallbackQuery query, Message message)
        {
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Branch Tank Report: {query.Data}");
        }
    }
}