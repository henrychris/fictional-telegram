using System;
using System.Threading.Tasks;
using app.Data.DTOs;
using app.Entities;
using app.Interfaces;
using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers.Helper
{
    public class BotControllerHelper : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEpumpDataRepository _epumpDataRepository;
        private readonly ILoginStatusRepository _loginStatusRepository;
        public BotControllerHelper(IMapper mapper, ILoggerManager logger, IUserRepository userRepository, IEpumpDataRepository epumpDataRepository,
        ILoginStatusRepository loginStatusRepository)
        {
            _loginStatusRepository = loginStatusRepository;
            _epumpDataRepository = epumpDataRepository;
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ActionResult> RegisterUser(TelegramUserDto data)
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

                // redirects To Page asking user to return to Telegram
                return Redirect("https://epump-login-test.herokuapp.com/");
            }
        }

        /// <summary>
        /// This method returns true or false depending on whether the 
        /// registration suceeds or not.
        /// </summary>
        public async Task<bool> RegisterEpumpUser(EpumpDataDto data)
        {
            if (data?.user.id == null) return false;

            var user = _mapper.Map<EpumpDataDto, EpumpData>(data);
            var check = await _loginStatusRepository.IsUserLoggedInAsync(user.ChatId, user.ID);

            if (check)
            {
                return false;
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

                return true;
            }
        }
    }
}