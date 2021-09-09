using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace app.Interfaces
{
    public interface IHandleUpdate
    {
        Task EchoAsync(Update update);
        public Task BotOnMessageReceived(Message message);
        public Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery);
        public Task UnknownUpdateHandler(Update update);
        Task<Message> UnknownCommand(Message message);
    }
}