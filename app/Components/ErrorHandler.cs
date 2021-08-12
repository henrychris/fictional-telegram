using System;
using System.Threading.Tasks;
using Contracts;
using Telegram.Bot.Exceptions;

namespace app.Components
{
    public class ErrorHandler
    {
        private readonly ILoggerManager _logger;
        public ErrorHandler(ILoggerManager logger)
        {
            _logger = logger;
        }

        public Task HandleErrorAsync(Exception exception)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogError(errorMessage);
            return Task.CompletedTask;
        }
    }
}