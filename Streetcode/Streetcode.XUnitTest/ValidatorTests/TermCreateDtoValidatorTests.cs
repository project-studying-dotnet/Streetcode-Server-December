using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Terms;
using Streetcode.BLL.Validation.Validators.DTOValidators.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class TermCreateDTOValidatorTests
    {
        private readonly TermCreateDTOValidator _validator;

        public TermCreateDTOValidatorTests()
        {
            _validator = new TermCreateDTOValidator();
        }

        [Fact]
        public void Should_HaveErrorWhenTitle_IsTooLong()
        {
            var longTitle = new string('a', 51);
            var dto = new TermCreateDTO { Title = longTitle };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Title)
                .WithErrorMessage("Title cannot exceed 50 characters.");
        }

        [Fact]
        public void Should_HaveErrorWhenDescription_IsTooLong()
        {
            var longDescription = new string('a', 501);
            var dto = new TermCreateDTO { Description = longDescription };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description cannot exceed 500 characters.");
        }

        [Fact]
        public void Should_NotHaveAnyErrorsForValidDTO()
        {
            var dto = new TermCreateDTO { Title = "Valid Title", Description = "Valid Description" };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
