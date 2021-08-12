using System;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using app.Entities;
using app.Interfaces;
using app.Components.Summary;
using System.Text.Json;
using System.IO;
using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;

namespace app.Components.Reports
{
    public class BranchReports
    {
        public HttpClient client;
        private readonly ITelegramBotClient _botClient;
        private readonly IEpumpDataRepository _epumpDataRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserRepository _userRepository;
        Stream Pdfreport = null;
        string dateTimeFormat = "MMM dd, yyyy";
        string endDate = DateTime.Today.ToString("MMM dd, yyyy");

        public BranchReports(ITelegramBotClient botClient, IHttpClientFactory httpClientFactory, IEpumpDataRepository epumpDataRepository,
        IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _epumpDataRepository = epumpDataRepository;
            _httpClientFactory = httpClientFactory;
            _botClient = botClient;
            client = _httpClientFactory.CreateClient("TestApi");
        }

        public string ConvertTextToDateTime(string dateTimeText)
        {
            // Epump's Date Format is "Jan 1st, 2021"
            // or "January 1, 2021"
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

        public async Task<Message> SendBranchCashflowReportAsync(CallbackQuery query, Message message)
        {
            var userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            var branchId = await _userRepository.GetCurrentBranchIdAsync(query.Message.Chat.Id);
            string uri = $"EpumpReport/Branch/BranchCashFlowReport?companyId={userData.CompanyId}&branchId={branchId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={endDate}";

            var content = await GetReportWithoutSummaryDataAsync(message, uri, userData);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"BranchCashflowReport.pdf"));
        }

        public async Task<Message> SendBranchSalesTransactionsReportAsync(CallbackQuery query, Message message)
        {
            var userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            var branchId = await _userRepository.GetCurrentBranchIdAsync(query.Message.Chat.Id);
            string uri = $"EpumpReport/Branch/BranchSalesTransactionReport?companyId={userData.CompanyId}&branchId={branchId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={endDate}";

            var content = await GetReportWithSummaryDataAsync(message, uri, userData);
            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary
            
Total PMS Sold: {content.pmsAmountSold}
Total AGO Sold: {content.agoAmountSold}
Total DPK Sold: {content.dpkAmountSold}
Total Amount Sold: {content.totalAmountSold}
Total PMS Volume Sold: {content.pmsVolumeSold}
Total AGO Volume Sold: {content.agoVolumeSold}
Total DPK Volume Sold: {content.dpkVolumeSold}
Total Volume Sold: {content.totalVolumeSold}
");

            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(Pdfreport, $"BranchSalesTransactions.pdf"));
        }

        public async Task<Message> SendBranchTankReportAsync(CallbackQuery query, Message message)
        {
            var userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            var branchId = await _userRepository.GetCurrentBranchIdAsync(query.Message.Chat.Id);
            string uri = $"EpumpReport/Branch/TankReport?companyId={userData.CompanyId}&branchId={branchId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={endDate}";

            var content = await GetReportWithoutSummaryDataAsync(message, uri, userData);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"BranchTankReport.pdf"));
        }

        public async Task<Message> SendBranchTanksFilledReportAsync(Message message)
        {
            var date = ValidateDateInput(message);
            if (date == "Invalid Date")
            {
                return await _botClient.SendTextMessageAsync(message.Chat.Id, "Please input time in this format: 'Jan 01, 2000'", replyMarkup: new ForceReplyMarkup());
            }

            var userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            var branchId = await _userRepository.GetCurrentBranchIdAsync(message.Chat.Id);
            string uri = $"EpumpReport/Branch/BranchTanksFilledReport?companyId={userData.CompanyId}&branchId={branchId}&date={date}";
            
            var result = await GetReportWithSummaryDataAsync(message, uri, userData);

            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary

Volume Discharged(Epump): {result.epumpDischarge}
Volume Discharged(Manual): {result.manualDischarge}            
");

            return await _botClient.SendDocumentAsync(message.Chat.Id, new InputOnlineFile(Pdfreport, $"BranchTanksFilledReport.pdf"));
        }

        public string ValidateDateInput(Message message)
        {
            DateTime date;

            if (DateTime.TryParseExact(message.Text, dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date.ToString(dateTimeFormat);
            }
            else
            {
                return "Invalid Date";
            }
        }

        public async Task<Message> SendBranchVarianceReportAsync(CallbackQuery query, Message message)
        {
            var userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            var branchId = await _userRepository.GetCurrentBranchIdAsync(query.Message.Chat.Id);
            string uri = $"EpumpReport/Branch/BranchVarianceReport?companyId={userData.CompanyId}&branchId={branchId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={endDate}";

            var content = await GetReportWithSummaryDataAsync(message, uri, userData);
            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary
Total Volume Sold(Epump): {content.totalEpumpVolumeSold}
Total Volume Sold(Manual): {content.totalManualVolumeSold}
Variance: {content.totalVariance}
");

            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(Pdfreport, $"BranchVarianceReport.pdf"));
        }

        public async Task<Message> SendProductSummaryReportAsync(CallbackQuery query, Message message)
        {
            var userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            var branchId = await _userRepository.GetCurrentBranchIdAsync(query.Message.Chat.Id);
            string uri = $"EpumpReport/Branch/BranchProductSummary?companyId={userData.CompanyId}&branchId={branchId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={endDate}";

            var content = await GetReportWithSummaryDataAsync(message, uri, userData);
            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary

PMS Sold: {content.pmsAmountSold}
AGO Sold: {content.agoAmountSold}
DPK Sold: {content.dpkAmountSold}
Total Amount Sold: {content.totalAmountSold}
PMS Volume Sold: {content.pmsVolumeSold}
AGO Volume Sold: {content.agoVolumeSold}
DPK Volume Sold: {content.dpkVolumeSold}
Total Volume Sold: {content.totalVolumeSold}
");

            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(Pdfreport, $"ProductSummaryReport.pdf"));
        }

        private async Task<SummaryData> GetReportWithSummaryDataAsync(Message message, string uri, EpumpData userData)
        {
            // this makes it a little less repetitive
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Sending PDF...");
            SummaryData result = null;

            var response = await client.GetAsync(uri);
            var content = await response.Content.ReadAsStreamAsync();
            result = await JsonSerializer.DeserializeAsync<SummaryData>(content);

            Pdfreport = new MemoryStream(result.pdfReport);
            return result;
        }

        private async Task<Stream> GetReportWithoutSummaryDataAsync( Message message, string uri, EpumpData userData)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Sending PDF...");

            var response = await client.GetAsync(uri);
            var content = await response.Content.ReadAsStreamAsync();
            return content;
        }
    }
}