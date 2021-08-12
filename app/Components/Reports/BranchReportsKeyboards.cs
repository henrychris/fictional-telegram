using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using app.Entities;
using app.Interfaces;

namespace app.Components.Reports
{
    public class BranchReportsKeyboards
    {
        public HttpClient client;
        private readonly ITelegramBotClient _botClient;
        private readonly IEpumpDataRepository _epumpDataRepository;
        private readonly IHttpClientFactory _httpClientFactory;
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

        public BranchReportsKeyboards(ITelegramBotClient botClient, IUserRepository userRepository, IHttpClientFactory httpClientFactory
        , IEpumpDataRepository epumpDataRepository)
        {
            _epumpDataRepository = epumpDataRepository;
            _httpClientFactory = httpClientFactory;
            _userRepository = userRepository;
            _botClient = botClient;
            client = _httpClientFactory.CreateClient("EpumpApi");
        }
        public async Task GetIdForBranchCashflowReport(Message message)
        {
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            await ShowBranchDataAsKeyboardAsync(message.Chat.Id);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchCashflowReportID");
        }

        public async Task<Message> SendBranchCashflowReportKeyboard(CallbackQuery query, Message message)
        {
            await _userRepository.SetCurrentBranchIdAsync(message.Chat.Id, query.Data);
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the cashflow report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchCashflowReport");
            return null;
        }

        public async Task GetIdForBranchSalesTransactionsReport(Message message)
        {
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            await ShowBranchDataAsKeyboardAsync(message.Chat.Id);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchSalesTransactionsReportID");
        }

        public async Task<Message> SendBranchSalesTransactionsReportKeyboard(CallbackQuery query, Message message)
        {
            await _userRepository.SetCurrentBranchIdAsync(message.Chat.Id, query.Data);
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Sales Transactions report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchSalesTransactionsReport");
            return null;
        }

        public async Task GetIdForBranchTankReport(Message message)
        {
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            await ShowBranchDataAsKeyboardAsync(message.Chat.Id);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchTankReportID");
        }

        public async Task<Message> SendBranchTankReportKeyboard(CallbackQuery query, Message message)
        {
            await _userRepository.SetCurrentBranchIdAsync(message.Chat.Id, query.Data);
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Tank report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchTankReport");
            return null;
        }

        public async Task GetIdForBranchTanksFilledReport(Message message)
        {
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            await ShowBranchDataAsKeyboardAsync(message.Chat.Id);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchTanksFilledReportID");
        }

        public async Task<Message> SendBranchTanksFilledReportKeyboard(CallbackQuery query, Message message)
        {
            await _userRepository.SetCurrentBranchIdAsync(message.Chat.Id, query.Data);

            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchTanksFilledReport");
            await _botClient.SendTextMessageAsync(message.Chat.Id, "Input time in this format: 'Jan 01, 2000'", replyMarkup: new ForceReplyMarkup());
            return null;
        }

        public async Task GetIdForBranchVarianceReport(Message message)
        {
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            await ShowBranchDataAsKeyboardAsync(message.Chat.Id);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchVarianceReportID");
        }

        public async Task<Message> SendBranchVarianceReportKeyboard(CallbackQuery query, Message message)
        {
            await _userRepository.SetCurrentBranchIdAsync(message.Chat.Id, query.Data);
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Variance report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchVarianceReport");
            return null;
        }

        public async Task GetIdForProductSummaryReport(Message message)
        {
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            await ShowBranchDataAsKeyboardAsync(message.Chat.Id);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchProductSummaryReportID");
        }

        public async Task<Message> SendProductSummaryReportKeyboard(CallbackQuery query, Message message)
        {
            await _userRepository.SetCurrentBranchIdAsync(message.Chat.Id, query.Data);
            // delete last message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Select a time range for the Product Summary report", replyMarkup: keyboard);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "BranchProductSummaryReport");
            return null;
        }

        private async Task ShowBranchDataAsKeyboardAsync(long ChatId)
        {
            // clear previous Id whenever the keyboard is displayedi
            await _userRepository.SetCurrentBranchIdToNull(ChatId);

            Dictionary<string, string> branchDataDictionary = new Dictionary<string, string>();
            var userDetails = await _epumpDataRepository.GetUserDetailsAsync(ChatId);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userDetails.AuthKey}");
            var companyBranchData = await client.GetAsync($"/Company/Branches/{userDetails.CompanyId}");

            var branchData = await JsonSerializer.DeserializeAsync<List<CompanyBranchInfo>>(companyBranchData.Content.ReadAsStreamAsync().Result);
            foreach (var branch in branchData)
            {
                branchDataDictionary.Add(branch.name, branch.id);
            }
            branchDataDictionary.Add("Back", "BranchReports");

            // selects and displays keyboard with branchId as callback data
            var BranchKeyboard = new InlineKeyboardMarkup(branchDataDictionary.Select(x => new[] { InlineKeyboardButton.WithCallbackData(x.Key, x.Value) }));
            await _botClient.SendTextMessageAsync(ChatId, "Select Branch", replyMarkup: BranchKeyboard);
        }
    }
}