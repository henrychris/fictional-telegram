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
        private readonly BranchReports _branchReports;

        public HandleUpdateService(ITelegramBotClient botClient, ILoggerManager logger,
        Keyboards keyboards, ErrorHandler errorHandler, BranchReports branchReports
        )
        {
            _branchReports = branchReports;
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
                "Cashflow" => _branchReports.SendCashflowReport(callbackQuery.Message),


                // responding to an unknown command
                _ => UnknownCommand(callbackQuery.Message),
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
            // await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, "Unknown command. Use /menu to get started.");
        }

        public Task UnknownUpdateHandler(Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            _logger.LogInfo($"Received unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
    }
}