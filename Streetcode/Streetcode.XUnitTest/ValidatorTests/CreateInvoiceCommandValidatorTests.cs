using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Payment;
using Streetcode.BLL.MediatR.Payment;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreateInvoiceCommandValidatorTests
    {
        private readonly CreateInvoiceCommandValidator _validator;

        public CreateInvoiceCommandValidatorTests()
        {
            _validator = new CreateInvoiceCommandValidator();
        }

        [Fact]
        public void Should_HaveErrorWhenPaymentNull()
        {
            // Arrange
            var command = new CreateInvoiceCommand(null);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Payment)
                  .WithErrorMessage("Payment cannot be null");
        }

        [Fact]
        public void Should_NotHaveErrorForValidPayment()
        {
            // Arrange
            var validPayment = new PaymentDto
            {
                Amount = 100,
                RedirectUrl = "https://example.com"
            };

            var command = new CreateInvoiceCommand(validPayment);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Payment);
        }

        [Fact]
        public void Should_InvokePaymentDTOValidatorForIncorrectPayment()
        {
            // Arrange
            var invalidPayment = new PaymentDto
            {
                Amount = -5,
                RedirectUrl = "https://example.com"
            };

            var command = new CreateInvoiceCommand(new PaymentDto());

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Payment.Amount)
                  .WithErrorMessage("Amount must be greater than 0");
        }
    }
}
