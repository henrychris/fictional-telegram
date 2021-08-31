using System.Threading.Tasks;
using app.Controllers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using app.Interfaces;

namespace app.Controllers
{

    [ApiController]
    [Route("/api/[controller]")]
    public class TestController : ControllerBase, ITestController
    {
        private readonly ILoginStatusRepository _loginStatusRepository;
        public TestController(ILoginStatusRepository loginStatusRepository)
        {
            _loginStatusRepository = loginStatusRepository;
        }

        [HttpGet("bad-request")]
        public IActionResult GetBadRequest()
        {
            return BadRequest();
        }

        [HttpGet("not-found")]
        public IActionResult GetNotFound()
        {
            return NotFound();
        }

        [HttpGet("server-error")]
        public IActionResult GetServerError()
        {
            return null;
        }

        [HttpGet("is-logged-in")]
        public async Task<IActionResult> GetIsLoggedIn(long chatId, string epumpId)
        {
            var check = await _loginStatusRepository.IsUserLoggedInAsync(chatId, epumpId);

            return (check) ? Ok() : NotFound();
        }

        [HttpGet("is-logged-in-telegram")]
        public async Task<IActionResult> GetIsLoggedInTelegram(long chatId)
        {
            var check = await _loginStatusRepository.IsUserLoggedInAsync_Telegram(chatId);
            return (check) ? Ok() : NotFound();
        }

        [HttpGet("is-logged-in-epump")]
        public async Task<IActionResult> GetIsLoggedInEpump(string epumpId)
        {
            var check = await _loginStatusRepository.IsUserLoggedInAsync_Epump(epumpId);
            return (check) ? Ok() : NotFound();
        }
    }
}