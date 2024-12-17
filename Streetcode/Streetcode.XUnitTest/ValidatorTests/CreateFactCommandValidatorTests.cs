using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreateFactCommandValidatorTests
    {
        private readonly CreateFactCommandValidator _validator;

        public CreateFactCommandValidatorTests()
        {
            _validator = new CreateFactCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Fact_Is_Null()
        {
            var model = new CreateFactCommand(null); 
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Fact);
        }

        [Fact]
        public void Should_Have_Error_When_Fact_Title_Is_Empty()
        {
            var factDto = new CreateFactDTO
            {
                Title = "",
                Text = "Valid text",
                ImageDescription = "Valid image description"
            };

            var model = new CreateFactCommand(factDto); 
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor("Fact.Title");
        }

        [Fact]
        public void Should_Have_Error_When_Fact_Text_Exceeds_Length()
        {
            var factDto = new CreateFactDTO
            {
                Title = "Valid Title",
                Text = new string('A', 601), 
                ImageDescription = "Valid description"
            };

            var model = new CreateFactCommand(factDto); 
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor("Fact.Text");
        }

        [Fact]
        public void Should_Not_Have_Error_For_Valid_Fact()
        {
            var factDto = new CreateFactDTO
            {
                Title = "Valid Title",
                Text = "Valid text within 600 characters.",
                ImageDescription = "Valid description within 200 characters."
            };

            var model = new CreateFactCommand(factDto); 
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
