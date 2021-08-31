using System;
using System.Threading.Tasks;
using app.Data.DTOs;
using app.Entities;
using app.Interfaces;
using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEpumpDataRepository _epumpDataRepository;
        private readonly ITelegramBotClient _botClient;
        private readonly ILoginStatusRepository _loginStatusRepository;

        public BotController(IMapper mapper, ILoggerManager logger, IUserRepository userRepository, IEpumpDataRepository epumpDataRepository,
        ITelegramBotClient botClient, ILoginStatusRepository loginStatusRepository)
        {
            _loginStatusRepository = loginStatusRepository;
            _botClient = botClient;
            _epumpDataRepository = epumpDataRepository;
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("register")]
        public async Task<ActionResult<TelegramUserDto>> RegisterUser([FromQuery] TelegramUserDto data)
        {
            if (data == null) return BadRequest("No data");

            var user = _mapper.Map<AppUser>(data);

            if (await _loginStatusRepository.IsUserLoggedInAsync_Telegram(user.ChatId))
            {
                return BadRequest("User already exists");
            }
            else
            {
                await _userRepository.AddUserAsync(user);
                await _loginStatusRepository.AddAsync(new LoginStatusTelegram
                {
                    UserChatId = user.ChatId,
                    LoginDate = DateTime.Now,
                    IsLoggedIn = true
                });

                _logger.LogInfo($"User {user.FirstName} with ID {user.ChatId} has been added to the database, at {DateTime.Now}");
                // redirects user to epump login page
                return Redirect("https://epump-login-test.herokuapp.com/");
            }
        }

        [HttpPost("register/epump")]
        public async Task<ActionResult> RegisterEpumpUser(EpumpDataDto data)
        {
            // If the epump Id(PK) is missing, it can't be added to the db anyway.
            if (data == null || data.EpumpId == null) return BadRequest("No data");

            var user = _mapper.Map<EpumpData>(data);
            var check = await _loginStatusRepository.IsUserLoggedInAsync(user.ChatId, user.ID);
            if (check)
            {
                return BadRequest("User already exists");
            }
            else
            {
                await _epumpDataRepository.AddUserAsync(user);
                _logger.LogInfo($"User {user.ChatId}, EpumpID: {user.ID} registered.");

                await _userRepository.FindAndUpdateUserWithEpumpDataAsync(user.ChatId, user.ID);
                _logger.LogInfo($"User {user.ChatId} database entry updated with EpumpID: {user.ID}");

                await _loginStatusRepository.AddAsync(new LoginStatusEpump
                {
                    EpumpDataId = user.ID,
                    LoginDate = DateTime.Now,
                    IsLoggedIn = true
                });
                _logger.LogInfo($"User {user.ID} has been fully added to the database, at {DateTime.Now}");
                return StatusCode(201);
            }
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
/*
    Regarding Authorization:
    I wish to use an OTP on the epump site to authorize the user, before updating the data.
    So, more work to do.
*/