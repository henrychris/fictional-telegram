using System;
using System.Threading.Tasks;
using app.Controllers.Helper;
using app.Data.DTOs;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace app.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BotController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly BotControllerHelper _botControllerHelper;

        public BotController(ILoggerManager logger,
        ITelegramBotClient botClient, BotControllerHelper botControllerHelper)
        {
            _botControllerHelper = botControllerHelper;
            _botClient = botClient;
            _logger = logger;
        }

        [HttpGet("register")]
        public async Task<ActionResult> RegisterUser([FromQuery] TelegramUserDto data)
        {
            return await _botControllerHelper.RegisterUser(data);
        }


        [HttpPost("notification")]
        public async Task<ActionResult> SendUserNotification([FromBody] NotificationDto data)
        {
            await _botClient.SendTextMessageAsync(data.ChatId, $"*Epump Notification*\n\n{data.Message}", parseMode: ParseMode.Markdown);
            _logger.LogInfo($"Notification sent to {data.ChatId} at{DateTime.Now}.");
            return Ok("Message Sent");
        }
    }
}