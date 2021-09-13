using app.Controllers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers
{

    [ApiController]
    [Route("/api/[controller]")]
    public class TestController : ControllerBase, ITestController
    {

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