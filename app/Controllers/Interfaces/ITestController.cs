using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers.Interfaces
{
    public interface ITestController
    {
        IActionResult GetBadRequest();
        IActionResult GetNotFound();
        IActionResult GetServerError();
        Task<IActionResult> GetIsLoggedIn(long chatId, string epumpId);
        Task<IActionResult> GetIsLoggedInTelegram(long chatId);
        Task<IActionResult> GetIsLoggedInEpump(string epumpId);
    }
}
