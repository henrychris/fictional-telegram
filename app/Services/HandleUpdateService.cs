using System;
using System.Linq;
using System.Threading.Tasks;
using app.Components;
using app.Components.Reports;
using app.Interfaces;
using Contracts;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace app.Services
{
    public class HandleUpdateService : Program, IHandleUpdate
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILoggerManager _logger;
        private readonly Keyboards _keyboards;
        private readonly ErrorHandler _errorHandler;
        private readonly BranchReportsKeyboards _branchReportsKeyboards;
        private readonly IUserRepository _userRepository;
        private readonly BranchReports _branchReports;
        private readonly CompanyReports _companyReports;
        private readonly CompanyReportsKeyboards _companyReportsKeyboards;

        public HandleUpdateService(ITelegramBotClient botClient, ILoggerManager logger,
        Keyboards keyboards, ErrorHandler errorHandler, BranchReportsKeyboards branchReportsKeyboards,
        IUserRepository userRepository, BranchReports branchReports, CompanyReports companyReports,
        CompanyReportsKeyboards companyReportsKeyboards
        )

        {
            _branchReports = branchReports;
            _companyReports = companyReports;
            _companyReportsKeyboards = companyReportsKeyboards;
            _userRepository = userRepository;
            _branchReportsKeyboards = branchReportsKeyboards;
            _errorHandler = errorHandler;
            _keyboards = keyboards;
            _botClient = botClient;
            _logger = logger;
        }

        public async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

            var action = (callbackQuery.Data) switch
            {
                "Menu" => _keyboards.SendMenu(callbackQuery.Message),
                "Login" => _keyboards.SendLoginKeyboard(callbackQuery.Message),
                "Reports" => _keyboards.SendReportKeyboard(callbackQuery.Message),

                // Branch Level
                "BranchReports" => _keyboards.SendBranchReportsKeyboard(callbackQuery.Message),

                // Company Level
                "CompanyReports" => _keyboards.SendCompanyReportsKeyboard(callbackQuery.Message),

                // answering specific report callback
                // Branch Reports
                "Cashflow" => _branchReportsKeyboards.SendBranchCashflowReportKeyboard(callbackQuery.Message),
                "Product Summary" => _branchReportsKeyboards.SendProductSummaryReportKeyboard(callbackQuery.Message),
                "Sales Transactions" => _branchReportsKeyboards.SendBranchSalesTransactionsReportKeyboard(callbackQuery.Message),
                "Tanks Filled" => _branchReportsKeyboards.SendBranchTanksFilledReportKeyboard(callbackQuery.Message),
                "Variance" => _branchReportsKeyboards.SendBranchVarianceReportKeyboard(callbackQuery.Message),
                "Tank Report" => _branchReportsKeyboards.SendBranchTankReportKeyboard(callbackQuery.Message),

                // Company Reports
                "Branch Sales" => _companyReportsKeyboards.SendCompanyBranchSalesReportKeyboard(callbackQuery.Message),
                "Company Cashflow" => _companyReportsKeyboards.SendCompanyCashflowReportKeyboard(callbackQuery.Message),
                "Sales Summary" => _companyReportsKeyboards.SendCompanySalesSummaryReportKeyboard(callbackQuery.Message),
                "Tank Stock" => _companyReportsKeyboards.SendCompanyTankStockReportKeyboard(callbackQuery.Message),
                "Company Tanks Filled" => _companyReportsKeyboards.SendCompanyTanksFilledReportKeyboard(callbackQuery.Message),
                "Company Variance" => _companyReportsKeyboards.SendCompanyVarianceReportKeyboard(callbackQuery.Message),
                "Expense Categories" => _companyReportsKeyboards.SendCompanyExpenseCategoriesReportKeyboard(callbackQuery.Message),
                "Zones Report" => _companyReportsKeyboards.SendCompanyZonesReportKeyboard(callbackQuery.Message),
                "Outstanding Payments" => _companyReportsKeyboards.SendCompanyOutstandingPaymentsReportKeyboard(callbackQuery.Message),
                "Wallet Report" => _companyReportsKeyboards.SendCompanyWalletReportKeyboard(callbackQuery.Message),
                "Retainership" => _companyReportsKeyboards.SendCompanyRetainershipReportKeyboard(callbackQuery.Message),
                "Wallet Fund Request" => _companyReportsKeyboards.SendCompanyWalletFundRequestReportKeyboard(callbackQuery.Message),

                // responding to an unknown command
                // _ => UnknownCommand(callbackQuery.Message),

                _ => CheckState(callbackQuery)
            };
        }

        public async Task BotOnMessageReceived(Message message)
        {
            // TODO add check for messages sent while Bot was offline
            if ((message.Date.ToLocalTime() < serverStart)) return;

            var action = (message.Text.Split(' ').First()) switch
            {
                "/start" => _keyboards.Start(message),
                "/menu" => _keyboards.SendMenu(message),
                "/help" => _keyboards.SendHelp(message),
                _ => UnknownCommand(message)
            };

            var sentMessage = await action;

            var log = $"The message was sent to {sentMessage.Chat.Id} with id: {sentMessage.MessageId}";
            System.Console.WriteLine(log);
            _logger.LogInfo(log);
        }


        // The hub. Where all updates go to first
        public async Task EchoAsync(Update update)
        {
            var handler = update.Type switch
            {
                // logic to handle all possible update types
                UpdateType.Message => BotOnMessageReceived(update.Message),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery),
                _ => UnknownUpdateHandler(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await _errorHandler.HandleErrorAsync(exception);
            }
        }

        public async Task<Message> UnknownCommand(Message message)
        {
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, "Unknown command. Use /menu to get started.");
        }

        public Task UnknownUpdateHandler(Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            _logger.LogInfo($"Received unknown update type: {update.Type}");
            return Task.CompletedTask;
        }

        public async Task<Message> CheckState(CallbackQuery query)
        {
            Message response = null;
            var userState = _userRepository.GetUserStateAsync(query.Message.Chat.Id);
            if (userState == null)
            {
                return await UnknownCommand(query.Message);
            }

            switch (userState)
            {
                case "BranchCashflowReport":
                    response = await _branchReports.SendBranchCashflowReportAsync(query, query.Message);
                    break;
                case "BranchProductSummaryReport":
                    response = await _branchReports.SendProductSummaryReportAsync(query, query.Message);
                    break;
                case "BranchSalesTransactionsReport":
                    response = await _branchReports.SendBranchSalesTransactionsReportAsync(query, query.Message);
                    break;
                case "BranchTanksFilledReport":
                    response = await _branchReports.SendBranchTanksFilledReportAsync(query, query.Message);
                    break;
                case "BranchVarianceReport":
                    response = await _branchReports.SendBranchVarianceReportAsync(query, query.Message);
                    break;
                case "BranchTankReport":
                    response = await _branchReports.SendBranchTankReportAsync(query, query.Message);
                    break;
                // Company Level
                case "CompanyBranchSalesReport":
                    response = await _companyReports.SendCompanyBranchSalesReportAsync(query, query.Message);
                    break;
                case "CompanyCashflowReport":
                    response = await _companyReports.SendCompanyCashflowReportAsync(query, query.Message);
                    break;
                case "CompanySalesSummaryReport":
                    response = await _companyReports.SendCompanySalesSummaryReportAsync(query, query.Message);
                    break;
                case "CompanyTankStockReport":
                    response = await _companyReports.SendCompanyTankStockReportAsync(query, query.Message);
                    break;
                case "CompanyVarianceReport":
                    response = await _companyReports.SendCompanyVarianceReportAsync(query, query.Message);
                    break;
                case "CompanyExpenseCategoriesReport":
                    response = await _companyReports.SendCompanyExpenseCategoriesReportAsync(query, query.Message);
                    break;
                case "CompanyZonesReport":
                    response = await _companyReports.SendCompanyZonesReportAsync(query, query.Message);
                    break;
                case "CompanyOutstandingPaymentsReport":
                    response = await _companyReports.SendCompanyOutstandingPaymentsReportAsync(query, query.Message);
                    break;
                case "CompanyWalletReport":
                    response = await _companyReports.SendCompanyWalletReportAsync(query, query.Message);
                    break;
                case "CompanyRetainershipReport":
                    response = await _companyReports.SendCompanyRetainershipReportAsync(query, query.Message);
                    break;
                case "CompanyWalletFundRequestReport":
                    response = await _companyReports.SendCompanyWalletFundRequestReportAsync(query, query.Message);
                    break;
                default:
                    response = await UnknownCommand(query.Message);
                    break;
            }
            return response;
        }
    }
}