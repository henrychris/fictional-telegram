using System;
using System.Linq;
using System.Threading.Tasks;
using app.Interfaces;
using Contracts;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace app.Services
{
    public class HandleUpdateService : IHandleUpdate
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILoggerManager _logger;

        public HandleUpdateService(ITelegramBotClient botClient, ILoggerManager logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

            var action = (callbackQuery.Data) switch
            {
                // "Menu" => Keyboards.SendMenu(callbackQuery.Message),
                // "Login" => Keyboards.SendLoginKeyboard(callbackQuery.Message),
                // "Reports" => Keyboards.SendReportKeyboard(callbackQuery.Message),
                // // Branch Level
                // "BranchReports" => Keyboards.SendBranchReportsKeyboard(callbackQuery.Message),

                // // Company Level
                // "CompanyReports" => Keyboards.SendCompanyReportsKeyboard(callbackQuery.Message),

                // // answering report day range request


                // // responding to an unknown command
                _ => UnknownCommand(callbackQuery.Message),
                // _ => SendMenu(callbackQuery.Message)
            };
        }

        public async Task BotOnMessageReceived(Message message)
        {
            // TODO add check for messages sent while Bot was offline

            var action = (message.Text.Split(' ').First()) switch
            {
                // "/start" => Keyboards.Start(message),
                // "/menu" => Keyboards.SendMenu(message),
                // "/help" => Keyboards.SendHelp(message),
                _ => UnknownCommand(message)
            };

            var sentMessage = await action;

            var log = $"The message was sent to {sentMessage.Chat.Id} with id: {sentMessage.MessageId}";
            System.Console.WriteLine(log);
            _logger.LogInfo(log);
        }

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
                await HandleErrorAsync(exception);
            }
        }

        public Task HandleErrorAsync(Exception exception)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogInfo(ErrorMessage);
            return Task.CompletedTask;
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
    }
}