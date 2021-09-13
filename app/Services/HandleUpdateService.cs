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
        private readonly EpumpLogin _epumpLogin;

        public HandleUpdateService(ITelegramBotClient botClient, ILoggerManager logger,
        Keyboards keyboards, ErrorHandler errorHandler, BranchReportsKeyboards branchReportsKeyboards,
        IUserRepository userRepository, BranchReports branchReports, CompanyReports companyReports,
        CompanyReportsKeyboards companyReportsKeyboards, EpumpLogin epumpLogin
        )

        {
            _branchReports = branchReports;
            _companyReports = companyReports;
            _companyReportsKeyboards = companyReportsKeyboards;
            _epumpLogin = epumpLogin;
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
                "DeleteUserData" => _keyboards.DeleteUserData(callbackQuery.Message),

                "EpumpLogin" => _epumpLogin.SendEpumpLoginKeyboard(callbackQuery.Message),

                // Branch Level
                "BranchReports" => _keyboards.SendBranchReportsKeyboard(callbackQuery.Message),

                // Company Level
                "CompanyReports" => _keyboards.SendCompanyReportsKeyboard(callbackQuery.Message),

                // answering specific report callback
                // Branch Reports
                "Cashflow" => _branchReportsKeyboards.GetIdForBranchCashflowReport(callbackQuery.Message),
                "Product Summary" => _branchReportsKeyboards.GetIdForProductSummaryReport(callbackQuery.Message),
                "Sales Transactions" => _branchReportsKeyboards.GetIdForBranchSalesTransactionsReport(callbackQuery.Message),
                "Tanks Filled" => _branchReportsKeyboards.GetIdForBranchTanksFilledReport(callbackQuery.Message),
                "Variance" => _branchReportsKeyboards.GetIdForBranchVarianceReport(callbackQuery.Message),
                "Tank Report" => _branchReportsKeyboards.GetIdForBranchTankReport(callbackQuery.Message),
                "Branch POS Transactions" => _branchReportsKeyboards.GetIdForBranchPosTransactionsReport(callbackQuery.Message),

                // Company Reports
                "Branch Sales" => _companyReportsKeyboards.SendCompanyBranchSalesReportKeyboard(callbackQuery.Message),
                "Company Cashflow" => _companyReportsKeyboards.SendCompanyCashflowReportKeyboard(callbackQuery.Message),
                "Sales Summary" => _companyReportsKeyboards.SendCompanySalesSummaryReportKeyboard(callbackQuery.Message),
                "Company Tanks Filled" => _companyReportsKeyboards.SendCompanyTanksFilledReportKeyboard(callbackQuery.Message),
                "Company Variance" => _companyReportsKeyboards.SendCompanyVarianceReportKeyboard(callbackQuery.Message),
                "Company POS Transactions" => _companyReportsKeyboards.SendCompanyPOSTransactionsReportKeyboard(callbackQuery.Message),

                // these don't require a date parameter
                "Expense Categories" => _companyReports.SendCompanyExpenseCategoriesReportAsync(callbackQuery, callbackQuery.Message),
                "Zones Report" => _companyReports.SendCompanyZonesReportAsync(callbackQuery, callbackQuery.Message),
                "Retainership" => _companyReports.SendCompanyRetainershipReportAsync(callbackQuery, callbackQuery.Message),
                "Wallet Report" => _companyReports.SendCompanyWalletReportAsync(callbackQuery, callbackQuery.Message),
                "Outstanding Payments" => _companyReports.SendCompanyOutstandingPaymentsReportAsync(callbackQuery, callbackQuery.Message),
                "Tank Stock" => _companyReports.SendCompanyTankStockReportAsync(callbackQuery, callbackQuery.Message),

                // this needs a status
                "Wallet Fund Request" => _companyReportsKeyboards.SendCompanyWalletFundRequestReportKeyboard(callbackQuery.Message),

                _ => CheckState(callbackQuery)
            };
        }

        public async Task BotOnMessageReceived(Message message)
        {
            if ((message.Date.ToLocalTime() < ServerStart)) return;

            var action = (message.Text.Split(' ').First()) switch
            {
                "/start" => _keyboards.Start(message),
                "/login" => _keyboards.SendLoginKeyboard(message),
                "/menu" => _keyboards.SendMenu(message),
                "/help" => _keyboards.SendHelp(message),
                "/reports" => _keyboards.SendReportKeyboard(message),
                "/delete" => _keyboards.SendConfirmationKeyboard(message),

                // Manages reports with unique inputs
                _ => CheckState(message)
            };

            var sentMessage = await action;

            var log = $"The message was sent to {sentMessage.Chat.Id} with id: {sentMessage.MessageId}";
            Console.WriteLine(log);
            _logger.LogInfo(log);
        }

        /// <summary>
        /// The hub. Where all updates go to first
        /// </summary>
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

        private async Task<Message> CheckState(Message message)
        {
            var userState = await _userRepository.GetUserStateAsync(message.Chat.Id);
            if (userState == null)
            {
                return await UnknownCommand(message);
            }

            var response = userState switch
            {
                // The report will be sent from here.
                "BranchTanksFilledReport" => await _branchReports.SendBranchTanksFilledReportAsync(message),
                "CompanyTanksFilledReport" => await _companyReports.SendCompanyTanksFilledReportAsync(message),
                "EpumpLogin_InputEmail" => await _epumpLogin.PromptUserForOtp(message),
                "EpumpLogin_ValidateOTP" => await _epumpLogin.ReceiveOtpFromUserInput(message),
                _ => await UnknownCommand(message)
            };
            return response;
        }

        private async Task<Message> CheckState(CallbackQuery query)
        {
            Message response;
            var userState = await _userRepository.GetUserStateAsync(query.Message.Chat.Id);
            if (userState == null)
            {
                return await UnknownCommand(query.Message);
            }

            switch (userState)
            {
                case "BranchCashflowReport":
                    response = await _branchReports.SendBranchCashflowReportAsync(query, query.Message);
                    break;
                case "BranchCashflowReportID":
                    response = await _branchReportsKeyboards.SendBranchCashflowReportKeyboard(query, query.Message);
                    break;

                case "BranchProductSummaryReport":
                    response = await _branchReports.SendProductSummaryReportAsync(query, query.Message);
                    break;
                case "BranchProductSummaryReportID":
                    response = await _branchReportsKeyboards.SendProductSummaryReportKeyboard(query, query.Message);
                    break;

                case "BranchSalesTransactionsReportID":
                    response = await _branchReportsKeyboards.SendBranchSalesTransactionsReportKeyboard(query, query.Message);
                    break;
                case "BranchSalesTransactionsReport":
                    response = await _branchReports.SendBranchSalesTransactionsReportAsync(query, query.Message);
                    break;

                case "BranchTanksFilledReportID":
                    response = await _branchReportsKeyboards.SendBranchTanksFilledReportKeyboard(query, query.Message);
                    break;

                case "BranchVarianceReport":
                    response = await _branchReports.SendBranchVarianceReportAsync(query, query.Message);
                    break;
                case "BranchVarianceReportID":
                    response = await _branchReportsKeyboards.SendBranchVarianceReportKeyboard(query, query.Message);
                    break;

                case "BranchTankReport":
                    response = await _branchReports.SendBranchTankReportAsync(query, query.Message);
                    break;
                case "BranchTankReportID":
                    response = await _branchReportsKeyboards.SendBranchTankReportKeyboard(query, query.Message);
                    break;
                case "BranchPOSTransactionsID":
                    response = await _branchReportsKeyboards.SendBranchPosTransactionsReportKeyboard(query, query.Message);
                    break;
                case "BranchPOSTransactions":
                    response = await _branchReports.SendBranchPosTransactionsReportAsync(query, query.Message);
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
                case "CompanyOutstandingPaymentsReport":
                    response = await _companyReports.SendCompanyOutstandingPaymentsReportAsync(query, query.Message);
                    break;
                case "CompanyRetainershipReport":
                    response = await _companyReports.SendCompanyRetainershipReportAsync(query, query.Message);
                    break;
                case "CompanyWalletFundRequestReport":
                    response = await _companyReports.SendCompanyWalletFundRequestReportAsync(query, query.Message);
                    break;
                case "CompanyPOSTransactionsReport":
                    response = await _companyReports.SendCompanyPosTransactionsReportAsync(query, query.Message);
                    break;
                default:
                    response = await UnknownCommand(query.Message);
                    break;
            }
            return response;
        }
    }
}