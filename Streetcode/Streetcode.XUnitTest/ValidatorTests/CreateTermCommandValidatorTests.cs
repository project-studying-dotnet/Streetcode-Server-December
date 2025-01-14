using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Terms;
using Streetcode.BLL.MediatR.Terms;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreateTermCommandValidatorTests
    {
        private readonly CreateTermCommandValidator _validator;

        public CreateTermCommandValidatorTests()
        {
            _validator = new CreateTermCommandValidator();
        }

        [Fact]
        public void Should_HaveErrorWhenTermCreateDTO_IsNull()
        {
            var command = new CreateTermCommand(null);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.TermCreateDTO)
                .WithErrorMessage("Term data cannot be null.");
        }
    }
}
