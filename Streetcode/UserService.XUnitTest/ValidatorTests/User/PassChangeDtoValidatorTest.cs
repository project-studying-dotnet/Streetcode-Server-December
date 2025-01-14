using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BLL.DTO.User;
using UserService.BLL.Validation;

namespace UserService.XUnitTest.ValidatorTests.User
{
    public class PassChangeDtoValidatorTest
    {
        private readonly PassChangeDtoValidator _validator;

        public PassChangeDtoValidatorTest()
        {
            _validator = new PassChangeDtoValidator();
        }

        [Fact]
        public void Validator_Should_Have_Error_When_Password_Is_Empty()
        {
            // Arrange
            var dto = new PassChangeDto
            {
                Password = string.Empty,
                PasswordConfirm = "validPassword",
                OldPassword = "validOldPassword"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password field is required.");
        }

        [Fact]
        public void Validator_Should_Have_Error_When_Password_Is_Too_Short()
        {
            // Arrange
            var dto = new PassChangeDto
            {
                Password = "123",
                PasswordConfirm = "validPassword",
                OldPassword = "validOldPassword"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password field must be at least 6 characters long.");
        }

        [Fact]
        public void Validator_Should_Have_Error_When_PasswordConfirm_Is_Empty()
        {
            // Arrange
            var dto = new PassChangeDto
            {
                Password = "validPassword",
                PasswordConfirm = string.Empty,
                OldPassword = "validOldPassword"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PasswordConfirm)
                  .WithErrorMessage("PasswordConfirm field is required.");
        }

        [Fact]
        public void Validator_Should_Have_Error_When_PasswordConfirm_Is_Too_Short()
        {
            // Arrange
            var dto = new PassChangeDto
            {
                Password = "validPassword",
                PasswordConfirm = "123",
                OldPassword = "validOldPassword"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PasswordConfirm)
                  .WithErrorMessage("PasswordConfirm field must be at least 6 characters long.");
        }

        [Fact]
        public void Validator_Should_Have_Error_When_OldPassword_Is_Empty()
        {
            // Arrange
            var dto = new PassChangeDto
            {
                Password = "validPassword",
                PasswordConfirm = "validPassword",
                OldPassword = string.Empty
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.OldPassword)
                  .WithErrorMessage("OldPassword field is required.");
        }

        [Fact]
        public void Validator_Should_Have_Error_When_OldPassword_Is_Too_Short()
        {
            // Arrange
            var dto = new PassChangeDto
            {
                Password = "validPassword",
                PasswordConfirm = "validPassword",
                OldPassword = "123"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.OldPassword)
                  .WithErrorMessage("OldPassword field must be at least 6 characters long.");
        }

        [Fact]
        public void Validator_Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            // Arrange
            var dto = new PassChangeDto
            {
                Password = "validPassword",
                PasswordConfirm = "validPassword",
                OldPassword = "validOldPassword"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
            result.ShouldNotHaveValidationErrorFor(x => x.PasswordConfirm);
            result.ShouldNotHaveValidationErrorFor(x => x.OldPassword);
        }
    }
}
