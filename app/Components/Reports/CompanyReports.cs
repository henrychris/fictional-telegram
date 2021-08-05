using System;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using app.Data.Repository;
using app.Entities;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using app.Interfaces;

namespace app.Components.Reports
{
    public class CompanyReports
    {
        public HttpClient client;
        private readonly ITelegramBotClient _botClient;
        private readonly IEpumpDataRepository _epumpDataRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        string dateTimeFormat = "MMM dd, yyyy";
        string endDate = DateTime.Today.ToString("MMM dd, yyyy");
        EpumpData userData;
        public CompanyReports(ITelegramBotClient botClient, IEpumpDataRepository epumpDataRepository, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _epumpDataRepository = epumpDataRepository;
            _botClient = botClient;
            client = _httpClientFactory.CreateClient("EpumpReportApi");
        }

        public string ConvertTextToDateTime(string dateTimeText)
        {
            // Epump's Date Format is "Jan 1st, 2021"
            switch (dateTimeText)
            {
                case "Today":
                    return DateTime.Today.ToString(dateTimeFormat);
                case "Yesterday":
                    return DateTime.Today.AddDays(-1).ToString(dateTimeFormat);
                case "Week":
                    return DateTime.Today.AddDays(-7).ToString(dateTimeFormat);
                case "Month":
                    return DateTime.Today.AddMonths(-1).ToString(dateTimeFormat);
                default:
                    return DateTime.Today.AddDays(-3).ToString(dateTimeFormat);
            }
        }

        // TODO
        /*
            receive callback data and run it through a switch to get the time frame for report.
            ! figure out how to store user company Ids.
            ? send typing action while pdf processes
        */

        public async Task<Message> SendCompanyBranchSalesReportAsync(CallbackQuery query, Message message)
        {
            // TODO specify the timeline in the report name
            // TODO add waiting message/typing action


            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            
                userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");

            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);
            var response = await client.GetAsync($"/EpumpReport/Company/CompanyBranchSalesReport?companyId={userData.CompanyId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={endDate}");
            var content = await response.Content.ReadAsStreamAsync();

            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanyBranchSalesReport.pdf"));
        }

        public async Task<Message> SendCompanyCashflowReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);

            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);

            var response = await client.GetAsync($"/EpumpReport/Company/CompanyCashflowReport?companyId={userData.CompanyId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={endDate}");
            var content = await response.Content.ReadAsStreamAsync();

            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanyCashFlowReport.pdf"));
        }

        public async Task<Message> SendCompanyExpenseCategoriesReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);

            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);

            var response = await client.GetAsync($"/EpumpReport/Company/Management/ExpenseCategoriesReport?companyId={userData.CompanyId}");
            var content = await response.Content.ReadAsStreamAsync();
            
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanyExpenseCategoriesReport.pdf"));
        }

        public async Task<Message> SendCompanyOutstandingPaymentsReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);

            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);

            var response = await client.GetAsync($"/EpumpReport/Company/Management/OutstandingPayments?companyId={userData.CompanyId}");
            var content = await response.Content.ReadAsStreamAsync();

            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanyOutstandingPaymentsReport.pdf"));
        }

        public async Task<Message> SendCompanyRetainershipReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);

            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);

            var response = await client.GetAsync($"/EpumpReport/Company/Retainerships/RetainershipReport?companyId={userData.CompanyId}");
            var content = await response.Content.ReadAsStreamAsync();

            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanyRetainershipReport.pdf"));
        }

        public async Task<Message> SendCompanySalesSummaryReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);

            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);

            var response = await client.GetAsync($"/EpumpReport/Company/CompanySalesSummaryReport?companyId={userData.CompanyId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={endDate}");
            var content = await response.Content.ReadAsStreamAsync();

            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanySalesSummaryReport.pdf"));
        }

        public async Task<Message> SendCompanyTankStockReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);

            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);

            var response = await client.GetAsync($"​/EpumpReport​/Company​/CompanyTankStockReport?companyId={userData.CompanyId}");
            var content = await response.Content.ReadAsStreamAsync();

            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanyTankStockReport.pdf"));
        }

        public async Task<Message> SendCompanyTanksFilledReportAsync(CallbackQuery query, Message message)
        {
            // TODO accept a single date
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company TanksFilled: {query.Data}\nNot Implemented");
        }

        public async Task<Message> SendCompanyVarianceReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            
            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);

            var response = await client.GetAsync($"​/EpumpReport​/Company​/CompanyVarianceReport?companyId={userData.CompanyId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={endDate}");
            var content = await response.Content.ReadAsStreamAsync();

            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanyVarianceReport.pdf"));
        }

        public async Task<Message> SendCompanyWalletFundRequestReportAsync(CallbackQuery query, Message message)
        {
            // TODO - accept companyId and status
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Company Wallet Fund Request: {query.Data}\nNot Implemented.");
        }

        public async Task<Message> SendCompanyWalletReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);

            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);

            var response = await client.GetAsync($"​/EpumpReport/Company/Management/CompanyWalletReport?companyId={userData.CompanyId}");
            var content = await response.Content.ReadAsStreamAsync();

            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanyWalletReport.pdf"));
        }

        public async Task<Message> SendCompanyZonesReportAsync(CallbackQuery query, Message message)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(query.Message.Chat.Id, message.MessageId);
            
            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);

            var response = await client.GetAsync($"​/EpumpReport/Company/Management/ZonesReport?companyId={userData.CompanyId}");
            var content = await response.Content.ReadAsStreamAsync();

            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanyZonesReport.pdf"));
        }
    }
}