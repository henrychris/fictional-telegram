using Xunit;
using app.Components;

namespace Tests.Login_RegisterTests
{
    public class EpumpLoginTests
    {
        private readonly EpumpLogin _epumpLogin;
        
        public EpumpLoginTests(EpumpLogin epumpLogin)
        {
            _epumpLogin = epumpLogin;
        }

        [Fact]
        public void ValidateEmail_GivenValidEmail_ReturnsTrue()
        {
            // Arrange
            var email = "ebenezer@mailinator.com";

            // Act
            var result = _epumpLogin.ValidateEmail(email);
            var expected = true;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ValidateEmail_GivenInvalidEmail_ReturnsFalse()
        {
            // Arrange
            var email = "ebenezer@mailinator";

            // Act
            var result = _epumpLogin.ValidateEmail(email);
            var expected = false;

            // Assert
            Assert.Equal(expected, result);
        }

        
    }
}