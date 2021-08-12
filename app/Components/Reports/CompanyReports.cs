using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using app.Components.Summary;
using app.Entities;
using app.Interfaces;
using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;

namespace app.Components.Reports
{
    public class CompanyReports
    {
        public HttpClient client;
        private readonly ITelegramBotClient _botClient;
        private readonly IEpumpDataRepository _epumpDataRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        Stream Pdfreport = null;
        string dateTimeFormat = "MMM dd, yyyy";
        string endDate = DateTime.Today.ToString("MMM dd, yyyy");
        EpumpData userData;
        public CompanyReports(ITelegramBotClient botClient, IEpumpDataRepository epumpDataRepository, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _epumpDataRepository = epumpDataRepository;
            _botClient = botClient;
            client = _httpClientFactory.CreateClient("TestApi");
        }

        public string ConvertTextToDateTime(string dateTimeText)
        {
            // TODO adjust the time settings
            // -7 might be more than a week
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

        public async Task<Message> SendCompanyBranchSalesReportAsync(CallbackQuery query, Message message)
        {
            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            string uri = $"EpumpReport/Company/CompanyBranchSalesReport?companyId={userData.CompanyId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={endDate}";

            var result = await GetReportWithSummaryDataAsync(message, uri, userData);

            await _botClient.SendTextMessageAsync(message.Chat.Id, $"Summary\nYou sold:\nPMS:{result.pmsAmount}\nAGO: {result.agoAmount}\nDPK: {result.dpkAmount}\nTotal: {result.totalAmount}");
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(Pdfreport, $"CompanyBranchSalesReport.pdf"));
        }

        public async Task<Message> SendCompanyCashflowReportAsync(CallbackQuery query, Message message)
        {
            // TODO this report hasn't been modified in ReportAPI
            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            string uri = $"EpumpReport/Company/CompanyCashflowReport?companyId={userData.CompanyId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={endDate}";
            var content = await GetReportWithoutSummaryDataAsync(message, uri, userData);

            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanyCashFlowReport.pdf"));
        }

        public async Task<Message> SendCompanyExpenseCategoriesReportAsync(CallbackQuery query, Message message)
        {
            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);

            string uri = $"EpumpReport/Company/Management/ExpenseCategoriesReport?companyId={userData.CompanyId}";
            var content = await GetReportWithoutSummaryDataAsync(message, uri, userData);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanyExpenseCategoriesReport.pdf"));
        }

        public async Task<Message> SendCompanyOutstandingPaymentsReportAsync(CallbackQuery query, Message message)
        {
            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            string uri = $"EpumpReport/Company/Management/OutstandingPayments?companyId={userData.CompanyId}";

            var result = await GetReportWithSummaryDataAsync(message, uri, userData);

            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary
Outstanding Amount: {result.outstandingAmount}");

            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(Pdfreport, $"CompanyOutstandingPaymentsReport.pdf"));
        }

        public async Task<Message> SendCompanyRetainershipReportAsync(CallbackQuery query, Message message)
        {
            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            string uri = $"EpumpReport/Company/Retainerships/RetainershipReport?companyId={userData.CompanyId}";

            var content = await GetReportWithoutSummaryDataAsync(message, uri, userData);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanyRetainershipReport.pdf"));
        }

        public async Task<Message> SendCompanySalesSummaryReportAsync(CallbackQuery query, Message message)
        {
            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);

            string uri = $"EpumpReport/Company/CompanySalesSummaryReport?companyId={userData.CompanyId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={endDate}";
            var result = await GetReportWithSummaryDataAsync(message, uri, userData);

            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary

PMS Tank Sales: {result.pmsTankSale}
AGO Tank Sales: {result.agoTankSale}
DPK Tank Sales: {result.dpkTankSale}
PMS Pump Sales: {result.pmsPumpSale}
AGO Pump Sales: {result.agoPumpSale}
DPK Pump Sales: {result.dpkPumpSale}
Total Sales: {result.totalAmount}
");
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(Pdfreport, $"CompanySalesSummaryReport.pdf"));
        }

        public async Task<Message> SendCompanyTankStockReportAsync(CallbackQuery query, Message message)
        {
            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            string uri = $"​EpumpReport​/Company​/CompanyTankStockReport?companyId={userData.CompanyId}";

            var result = await GetReportWithSummaryDataAsync(message, uri, userData);
            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary

PMS Volume: {result.pmsVolume}
AGO Volume: {result.agoVolume}
LPG Volume: {result.dpkVolume}
DPK Volume: {result.lpgVolume}
");
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(Pdfreport, $"CompanyTankStockReport.pdf"));
        }

        public async Task<Message> SendCompanyTanksFilledReportAsync( Message message)
        {
            var date = ValidateDateInput(message);
            if (date == "Invalid Date")
            {
                return await _botClient.SendTextMessageAsync(message.Chat.Id, "Please input time in this format: 'Jan 01, 2000'", replyMarkup: new ForceReplyMarkup());
            }

            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);

            string uri = $"EpumpReport/Company/CompanyTanksFilledReport?companyId={userData.CompanyId}&date={date}";
            var result = await GetReportWithSummaryDataAsync(message, uri, userData);
            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary

Volume Discharged(Epump): {result.epumpDischarge}
Volume Discharged(Manual): {result.manualDischarge}
");

            return await _botClient.SendDocumentAsync(message.Chat.Id, new InputOnlineFile(Pdfreport, $"CompanyTanksFilledReport.pdf"));
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

        public async Task<Message> SendCompanyVarianceReportAsync(CallbackQuery query, Message message)
        {
            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            string uri = $"​EpumpReport​/Company​/CompanyVarianceReport?companyId={userData.CompanyId}&startDate={ConvertTextToDateTime(query.Data)}&endDate={endDate}";

            var result = await GetReportWithSummaryDataAsync(message, uri, userData);

            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary

Total Volume Sold(Epump): {result.totalEpumpVolumeSold}L
Total Volume Sold(Manual): {result.totalManualVolumeSold}L
Variance: {result.totalVariance}L");
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(Pdfreport, $"CompanyVarianceReport.pdf"));
        }

        public async Task<Message> SendCompanyWalletFundRequestReportAsync(CallbackQuery query, Message message)
        {
            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            
            string uri = $"​EpumpReport/Company/Retainerships/WalletFundRequestReport?companyId={userData.CompanyId}&status={query.Data}";
            try
            {
                var result = await GetReportWithSummaryDataAsync(message, uri, userData);
                await _botClient.SendTextMessageAsync(query.Message.Chat.Id, $"Summary\nAmount Paid: {result.amountPaid}");
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(Pdfreport, $"WalletFundRequest.pdf"));
        }

        public async Task<Message> SendCompanyWalletReportAsync(CallbackQuery query, Message message)
        {
            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            string uri = $"​EpumpReport/Company/Management/CompanyWalletReport?companyId={userData.CompanyId}";

            var result = await GetReportWithSummaryDataAsync(message, uri, userData);
            await _botClient.SendTextMessageAsync(message.Chat.Id, $@"Summary

Wallet Balance: ₦{result.walletBalance}
Wallet BookBalance: ₦{result.walletBookBalance}
");
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(Pdfreport, $"CompanyWalletReport.pdf"));
        }

        public async Task<Message> SendCompanyZonesReportAsync(CallbackQuery query, Message message)
        {
            // TODO Custom Exception Middleware
            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);

            string uri = $"EpumpReport/Company/Management/ZonesReport?companyId={userData.CompanyId}";
            var content = await GetReportWithoutSummaryDataAsync(message, uri, userData);
            return await _botClient.SendDocumentAsync(query.Message.Chat.Id, new InputOnlineFile(content, $"CompanyZonesReport.pdf"));
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

        private async Task<Stream> GetReportWithoutSummaryDataAsync(Message message, string uri, EpumpData userData)
        {
            // delete the message
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            userData = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userData.AuthKey}");

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Sending PDF...");

            var response = await client.GetAsync(uri);
            var content = await response.Content.ReadAsStreamAsync();
            return content;
        }
    }
}