using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using app.Components.Summary;
using app.Entities;
using app.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace app.Components.Reports
{
    public class BranchReports
    {
        private readonly HttpClient _client;
        private readonly ITelegramBotClient _botClient;
        private readonly IEpumpDataRepository _epumpDataRepository;
        private readonly IUserRepository _userRepository;
        private Stream _pdfReport;
        private const string DateTimeFormat = "MMMM dd, yyyy";
        private readonly string _endDate = DateTime.Today.ToString("MMMM dd, yyyy");

        public BranchReports(ITelegramBotClient botClient, IHttpClientFactory httpClientFactory, IEpumpDataRepository epumpDataRepository,
        IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _epumpDataRepository = epumpDataRepository;
            _botClient = botClient;
            _client = httpClientFactory.CreateClient("TestReportApi");
        }

        private static string ConvertTextToDateTime(string dateTimeText)
        {
            // Epump's Date Format is "Jan 1st, 2021"
            // or "January 1, 2021"
            return dateTimeText switch
            {
                "Today" => DateTime.Today.ToString(DateTimeFormat),
                "Yesterday" => DateTime.Today.AddDays(-1).ToString(DateTimeFormat),
                "Week" => DateTime.Today.AddDays(-7).ToString(DateTimeFormat),
                "Month" => DateTime.Today.AddMonths(-1).ToString(DateTimeFormat),
                _ => DateTime.Today.AddDays(-3).ToString(DateTimeFormat)
            };
        }

        public async Task<Message> SendBranchCashflowReportAsync(CallbackQuery query, Message message)
        {
            var userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            var branchId = await _userRepository.GetCurrentBranchIdAsync(query.Message.Chat.Id);
            var uri = $"EpumpReport/Branch/BranchCashFlowReport?companyId={userData.CompanyId}&branchId={branchId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={_endDate}";

            var content = await GetReportWithoutSummaryDataAsync(message, uri, userData);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, "BranchCashflowReport.pdf"));
        }

        public async Task<Message> SendBranchSalesTransactionsReportAsync(CallbackQuery query, Message message)
        {
            var userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            var branchId = await _userRepository.GetCurrentBranchIdAsync(query.Message.Chat.Id);
            string uri = $"EpumpReport/Branch/BranchSalesTransactionReport?companyId={userData.CompanyId}&branchId={branchId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={_endDate}";

            var content = await GetReportWithSummaryDataAsync(message, uri, userData);
            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary
            
Total PMS Sold: {content.PmsAmountSold}
Total AGO Sold: {content.AgoAmountSold}
Total DPK Sold: {content.DpkAmountSold}
Total Amount Sold: {content.TotalAmountSold}
Total PMS Volume Sold: {content.PmsVolumeSold}
Total AGO Volume Sold: {content.AgoVolumeSold}
Total DPK Volume Sold: {content.DpkVolumeSold}
Total Volume Sold: {content.TotalVolumeSold}
");

            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(_pdfReport, "BranchSalesTransactions.pdf"));
        }

        public async Task<Message> SendBranchTankReportAsync(CallbackQuery query, Message message)
        {
            var userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            var branchId = await _userRepository.GetCurrentBranchIdAsync(query.Message.Chat.Id);
            string uri = $"EpumpReport/Branch/TankReport?companyId={userData.CompanyId}&branchId={branchId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={_endDate}";

            var content = await GetReportWithoutSummaryDataAsync(message, uri, userData);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, "BranchTankReport.pdf"));
        }

        public async Task<Message> SendBranchPosTransactionsReportAsync(CallbackQuery query, Message message)
        {
            var userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            var branchId = await _userRepository.GetCurrentBranchIdAsync(query.Message.Chat.Id);
            string uri = $"EpumpReport/PosTransactionReport?companyId={userData.CompanyId}&branchId={branchId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={_endDate}";

            var result = await GetReportWithSummaryDataAsync(message, uri, userData);
            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary
Amount of cashbacks : {result.CashbackAmount}
Failed Transactions: {result.FailedAmount}
Succesful Transactions: {result.SuccesfulAmount}
");
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(_pdfReport, "POS Transactions Report.pdf"));
        }   

        public async Task<Message> SendBranchTanksFilledReportAsync(Message message)
        {
            var date = ValidateDateInput(message);
            if (date == "Invalid Date")
            {
                return await _botClient.SendTextMessageAsync(message.Chat.Id, "Please input time in this format: 'January 01, 2000'", replyMarkup: new ForceReplyMarkup());
            }

            var userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            var branchId = await _userRepository.GetCurrentBranchIdAsync(message.Chat.Id);
            string uri = $"EpumpReport/Branch/BranchTanksFilledReport?companyId={userData.CompanyId}&branchId={branchId}&date={date}";
            
            var result = await GetReportWithSummaryDataAsync(message, uri, userData);

            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary

Volume Discharged(Epump): {result.EpumpDischarge}
Volume Discharged(Manual): {result.ManualDischarge}            
");

            return await _botClient.SendDocumentAsync(message.Chat.Id, new InputOnlineFile(_pdfReport, "BranchTanksFilledReport.pdf"));
        }

        private string ValidateDateInput(Message message)
        {
            return DateTime.TryParseExact(message.Text, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date.ToString(DateTimeFormat) : "Invalid Date";
        }

        public async Task<Message> SendBranchVarianceReportAsync(CallbackQuery query, Message message)
        {
            var userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            var branchId = await _userRepository.GetCurrentBranchIdAsync(query.Message.Chat.Id);
            string uri = $"EpumpReport/Branch/BranchVarianceReport?companyId={userData.CompanyId}&branchId={branchId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={_endDate}";

            var content = await GetReportWithSummaryDataAsync(message, uri, userData);
            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary
Total Volume Sold(Epump): {content.TotalEpumpVolumeSold}
Total Volume Sold(Manual): {content.TotalManualVolumeSold}
Variance: {content.TotalVariance}
");

            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(_pdfReport, "BranchVarianceReport.pdf"));
        }

        public async Task<Message> SendProductSummaryReportAsync(CallbackQuery query, Message message)
        {
            var userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            var branchId = await _userRepository.GetCurrentBranchIdAsync(query.Message.Chat.Id);
            string uri = $"EpumpReport/Branch/BranchProductSummary?companyId={userData.CompanyId}&branchId={branchId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={_endDate}";

            var content = await GetReportWithSummaryDataAsync(message, uri, userData);
            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary

PMS Sold: {content.PmsAmountSold}
AGO Sold: {content.AgoAmountSold}
DPK Sold: {content.DpkAmountSold}
Total Amount Sold: {content.TotalAmountSold}
PMS Volume Sold: {content.PmsVolumeSold}
AGO Volume Sold: {content.AgoVolumeSold}
DPK Volume Sold: {content.DpkVolumeSold}
Total Volume Sold: {content.TotalVolumeSold}
");

            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(_pdfReport, "ProductSummaryReport.pdf"));
        }

        private async Task<SummaryData> GetReportWithSummaryDataAsync(Message message, string uri, EpumpData userData)
        {
            // this makes it a little less repetitive
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Generating PDF...");
            
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await _client.SendAsync(requestMessage);

            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<SummaryData>(content);

            // TODO add handler for null reference exception
            _pdfReport = new MemoryStream(result.PdfReport);
            return result;
        }

        private async Task<Stream> GetReportWithoutSummaryDataAsync( Message message, string uri, EpumpData userData)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Generating PDF...");

            var response = await _client.GetAsync(uri);
            var content = await response.Content.ReadAsStreamAsync();
            return content;
        }
    }
}