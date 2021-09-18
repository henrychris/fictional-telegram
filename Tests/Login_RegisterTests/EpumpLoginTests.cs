using Xunit;
using app.Data;
using Moq;

namespace Tests.Login_RegisterTests
{
    public class EpumpLoginTests
    {
        private readonly Mock<IEpumpLogin> epumpLogin;
        
        public EpumpLoginTests()
        {
            epumpLogin = new Mock<IEpumpLogin>();

            epumpLogin.Setup(x => x.ValidateEmail(It.IsAny<string>()))
            .Returns((string x) => new System.Net.Mail.MailAddress(x).Address == x);
        }

        [Fact]
        public void ValidateEmail_GivenValidEmail_ReturnsTrue()
        {
            // Arrange
            var email = "ebenezer@gmail.com";

            // Act
            var result = epumpLogin.Object.ValidateEmail(email);
            var expected = true;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ValidateEmail_GivenInvalidEmail_ReturnsFormatException()
        {
            var email = "ebenezermailinator.x";

            Assert.Throws<System.FormatException>(() => epumpLogin.Object.ValidateEmail(email));
        }
    }
}