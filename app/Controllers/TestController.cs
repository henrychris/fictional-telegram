using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using app.Interfaces;

namespace app.Controllers
{

    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public TestController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
    }
}