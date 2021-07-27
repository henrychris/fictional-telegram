using System;
using System.Threading;
using System.Threading.Tasks;
using app.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace app.Components
{
    public class Keyboards
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UserActionsService _userActions;
        private readonly ErrorHandler _errorHandler;
        public Keyboards(ITelegramBotClient botClient, UserActionsService userActions
        , ErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
            _userActions = userActions;
            _botClient = botClient;
        }
    public async Task<Message> Start(Message message)
    {
        // TODO check for user existence
        // var user = await _userActions.DoesUserExist(message);
        // try
        // {
        //     if (user != null)
        //     {
        //         var response = await _botClient.SendTextMessageAsync(message.Chat.Id, $"Hello, {user.FirstName}!\nUse /menu to get started",
        //             replyMarkup: new ReplyKeyboardRemove());
        //         return response;
        //     }
        // }
        // catch (Exception exception)
        // {
        //     await _handler.HandleErrorAsync(exception);
        // }

        return await _botClient.SendTextMessageAsync(message.Chat.Id, "Hello, you are currently not logged in!\nUse /menu to get started.");
    }
}
}