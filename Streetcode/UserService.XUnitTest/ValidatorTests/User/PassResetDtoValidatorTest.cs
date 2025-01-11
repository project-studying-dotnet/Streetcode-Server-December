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
    public class PassResetDtoValidatorTest
    {
        private readonly PassResetDtoValidator _validator;

        public PassResetDtoValidatorTest()
        {
            _validator = new PassResetDtoValidator();
        }

        [Fact]
        public void Validator_Should_HaveErrors_When_EmailIsEmpty()
        {
            // Arrange
            var dto = new PassResetDto
            {
                Email = "",
                Password = "validPass123",
                Code = "validCode"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email field is required.");
        }

        [Fact]
        public void Validator_Should_HaveErrors_When_EmailIsInvalid()
        {
            // Arrange
            var dto = new PassResetDto
            {
                Email = "invalid-email",
                Password = "validPass123",
                Code = "validCode"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email field should be a real Email");
        }

        [Fact]
        public void Validator_Should_HaveErrors_When_PasswordIsEmpty()
        {
            // Arrange
            var dto = new PassResetDto
            {
                Email = "test@example.com",
                Password = "",
                Code = "validCode"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password field is required.");
        }

        [Fact]
        public void Validator_Should_HaveErrors_When_PasswordIsTooShort()
        {
            // Arrange
            var dto = new PassResetDto
            {
                Email = "test@example.com",
                Password = "123",
                Code = "validCode"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password field must be at least 6 characters long.");
        }

        [Fact]
        public void Validator_Should_HaveErrors_When_CodeIsEmpty()
        {
            // Arrange
            var dto = new PassResetDto
            {
                Email = "test@example.com",
                Password = "validPass123",
                Code = ""
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Code)
                  .WithErrorMessage("Code field is required.");
        }

        [Fact]
        public void Validator_Should_NotHaveErrors_When_AllFieldsAreValid()
        {
            // Arrange
            var dto = new PassResetDto
            {
                Email = "test@example.com",
                Password = "validPass123",
                Code = "validCode"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
            result.ShouldNotHaveValidationErrorFor(x => x.Code);
        }
    }
}
