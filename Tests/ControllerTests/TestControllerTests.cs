using System.Threading.Tasks;
using app.Controllers;
using app.Controllers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.ControllerTests
{
    public class TestControllerTests
    {
        private readonly TestController _testController;
        
        public TestControllerTests()
        {
            _testController = new TestController();
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
    }
}
