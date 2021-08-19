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

        public BotController(IMapper mapper, ILoggerManager logger, IUserRepository userRepository, IEpumpDataRepository epumpDataRepository,
        ITelegramBotClient botClient)
        {
            _botClient = botClient;
            _epumpDataRepository = epumpDataRepository;
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
        }

        // TODO
        /*
        Rewrite the user check method. Check both databases.
        However. If the login method performs this check,
        it won't be needed here.
        Maybe, perform checks in both DBs. 
        That way info can be updated wherever it is missing.
        */

        [HttpPost("/register")]
        public async Task<ActionResult<TelegramUserDto>> RegisterUser([FromQuery] TelegramUserDto data)
        {
            if (data == null) return BadRequest("No data");

            var user = _mapper.Map<AppUser>(data);

            if (await _userRepository.CheckUserExistsAsync(user.ChatId))
            {
                return BadRequest("User already exists");
            }
            else
            {
                await _userRepository.AddUserAsync(user);
                // redirects user to epump login page
                return Redirect("https://epump-login-test.herokuapp.com/");
            }
        }

        [HttpPost("/register/epump")]
        public async Task<ActionResult> RegisterEpumpUser(EpumpDataDto data)
        {
            // If the epump Id(PK) is missing, it can't be added to the db anyway.
            if (data == null || data.EpumpId == null) return BadRequest("No data");

            var user = _mapper.Map<EpumpData>(data);
            var check = await _epumpDataRepository.CheckUserExistsAsync(user.ID);
            if (check)
            {
                return BadRequest("User already exists");
            }
            else
            {
                await _epumpDataRepository.AddUserAsync(user);
                await _userRepository.FindAndUpdateUserWithEpumpDataAsync(user.ChatId, user.ID);
                return Ok("Success");
            }
        }

        [HttpPost("/notification")]
        public async Task<ActionResult> SendUserNotification([FromBody] NotificationDto data)
        {
            try
            {
                await _botClient.SendTextMessageAsync(data.ChatId, $"*Epump Notification*\n\n{data.Message}", parseMode: ParseMode.Markdown);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
            return Ok("Message Sent");
        }
    }
}
/*
    Regarding Authorization:
    I wish to use an OTP on the epump site to authorize the user, before updating the data.
    So, more work to do.
*/