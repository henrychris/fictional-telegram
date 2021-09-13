using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers.Interfaces
{
    public interface ITestController
    {
        IActionResult GetBadRequest();
        IActionResult GetNotFound();
        IActionResult GetServerError();
    }
}
