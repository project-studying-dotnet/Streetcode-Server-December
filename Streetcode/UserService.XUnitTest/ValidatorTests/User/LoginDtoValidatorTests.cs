using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BLL.Validation;
using UserService.BLL.DTO.User;
using FluentValidation.TestHelper;

namespace UserService.XUnitTest.ValidatorTests.User
{
    public class LoginDtoValidatorTests
    {
        private readonly LoginDtoValidator _validator;

        public LoginDtoValidatorTests()
        {
            _validator = new LoginDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_UserName_Is_Empty()
        {
            // Arrange
            var dto = new LoginDTO { UserName = "" };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserName)
                  .WithErrorMessage("UserName is required.");
        }

        [Fact]
        public void Should_Have_Error_When_UserName_Is_Short()
        {
            // Arrange
            var dto = new LoginDTO { UserName = "ab" };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserName)
                  .WithErrorMessage("UserName must be between 3 and 50 characters.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Valid()
        {
            // Arrange
            var dto = new LoginDTO
            {
                UserName = "ValidUser",
                FullName = "Valid Full Name",
                Email = "user@example.com",
                Password = "password123!"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
