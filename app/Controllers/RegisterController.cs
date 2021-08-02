using System.Threading.Tasks;
using app.Data.DTOs;
using app.Data.Repository;
using app.Entities;
using app.Interfaces;
using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;

        public RegisterController(IMapper mapper, ILoggerManager logger, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("/register")]
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
                return Ok("Success");
            }
        }
    }
}