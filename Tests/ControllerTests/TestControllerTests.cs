using System.Threading.Tasks;
using app.Controllers;
using app.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.ControllerTests
{
    public class TestControllerTests
    {
        private readonly TestController _testController;
        private readonly Mock<ILoginStatusRepository> _loginStatusMock;
        public TestControllerTests()
        {
            _loginStatusMock = new Mock<ILoginStatusRepository>();
            _testController = new TestController(_loginStatusMock.Object);
        }

        [Fact]
        public void CheckIfBadRequestIsReturned()
        {
            var result = _testController.GetBadRequest();

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CheckIfNotFoundIsReturned()
        {
            var result = _testController.GetNotFound();

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CheckIfServerErrorIsReturned()
        {
            var result = _testController.GetServerError();

            // Returning null from a method of type action result results in an error 500.
            Assert.Null(result);
        }

        [Fact]
        public async Task GetIsLoggedIn_GivenChatAndEpumpId_ReturnsOkResult()
        {
            _loginStatusMock.Setup(t => t.IsUserLoggedInAsync(It.Is<long>(i => i == 926122532), It.Is<string>(i => i == "1")))
               .ReturnsAsync(true);

           // Test Controller function `GetUserLoggedIn` calls the loginStatusRepo function `IsUserLoggedIn`.
           // By mocking it such that it checks params for the values that would be in the db
           // I can test if a certain user is logged in.

           var result = await _testController.GetIsLoggedIn(926122532, "1");
           Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetIsLoggedInTelegram_GivenChatId_ReturnsOkResult()
        {
            _loginStatusMock.Setup(t => t.IsUserLoggedInAsync_Telegram(It.Is<long>(i => i == 926122532)))
                .ReturnsAsync(true);

            var result = await _testController.GetIsLoggedInTelegram(926122532);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetIsLoggedInEpump_GivenEpumpId_ReturnsOkResult()
        {
            _loginStatusMock.Setup(t => t.IsUserLoggedInAsync_Epump(It.Is<string>(i => i == "1")))
                .ReturnsAsync(true);

            var result = await _testController.GetIsLoggedInEpump("1");
            Assert.IsType<OkResult>(result);
        }
    }
}
