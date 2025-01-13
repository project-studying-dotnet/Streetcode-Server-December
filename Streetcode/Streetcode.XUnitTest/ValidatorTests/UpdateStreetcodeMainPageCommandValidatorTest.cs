using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.CreateMainPage;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.UpdateMainPage;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class UpdateStreetcodeMainPageCommandValidatorTest
    {
        private readonly UpdateStreetcodeMainPageCommandValidator _validator;
        private readonly List<ImageFileBaseCreateDto> imageListEmpty = new List<ImageFileBaseCreateDto> { };

        public UpdateStreetcodeMainPageCommandValidatorTest()
        {
            _validator = new UpdateStreetcodeMainPageCommandValidator();
        }

        [Fact]
        public void Validator_Should_HaveError_When_IdIsEmpty()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.Id)
                .WithErrorMessage("The 'Id' field is required");
        }

        [Fact]
        public void Id_ShouldBeValid_WhenNotEmpty()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                Id = 1,
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.Id);
        }

        [Fact]
        public void Validator_Should_HaveError_When_DateStringIsEmpty()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                DateString = "",
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.DateString)
                .WithErrorMessage("The 'Date String' field is required");
        }

        [Fact]
        public void Validator_Should_HaveError_When_DateStringIsLongerThan50Characters()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                DateString = "hcgyux4r,gfehmdgtw9gkuefhldgtiu589t;gietfuldioeufkhlkyuhfe89t8hkul9ed8thuk9658hkul9ehtld",
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.DateString)
                .WithErrorMessage("The Date String cannot be longer than 50 characters");
        }

        [Fact]
        public void DateString_ShouldBeValid_WhenNotEmptyAndLessThan50Characters()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                DateString = "hcgyux4r,gfehmdgtw9gkuefhld",
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.DateString);
        }

        [Fact]
        public void Index_ShouldBeValid_WhenNotEmptyAndGreaterThanOrEqualToZero()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                Index = 1,
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.Index);
        }

        [Fact]
        public void Index_ShouldHaveValidationError_WhenEmptyOrNegative()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                Index = -1,
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.Index)
                .WithErrorMessage("Index must be greater than or equal to 0");
        }

        [Fact]
        public void Title_ShouldHaveValidationError_WhenExceedsMaxLength()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                Title = new string('a', 101),
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.Title)
                .WithErrorMessage("The Title cannot be longer than 100 characters");
        }

        [Fact]
        public void FirstName_ShouldHaveValidationError_WhenExceedsMaxLength()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                FirstName = new string('a', 51),
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.FirstName)
                .WithErrorMessage("The First Name cannot be longer than 50 characters");
        }

        [Fact]
        public void LastName_ShouldHaveValidationError_WhenExceedsMaxLength()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                LastName = new string('a', 51),
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.LastName)
                .WithErrorMessage("The Last Name cannot be longer than 50 characters");
        }

        [Fact]
        public void Teaser_ShouldHaveValidationError_WhenExceedsMaxLength()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                Teaser = new string('a', 451),
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.Teaser)
                .WithErrorMessage("The Teaser cannot be longer than 450 characters");
        }

        [Fact]
        public void EventStartOrPersonBirthDate_ShouldHaveValidationError_WhenEmpty()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto 
            { 
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.EventStartOrPersonBirthDate)
                .WithErrorMessage("The 'Event Start Or Person Birth Date' field is required");
        }

        [Fact]
        public void TransliterationUrl_ShouldHaveValidationError_WhenInvalid()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                TransliterationUrl = "Invalid_Url",
                Images = imageListEmpty
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.TransliterationUrl)
                .WithErrorMessage("Transliteration Url is allowed to accept lowercase latin letters, numbers, and hyphens.");
        }

        [Fact]
        public void TransliterationUrl_ShouldNotHaveValidationError_WhenValid()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                TransliterationUrl = "valid-url-123",
                Images = imageListEmpty
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.TransliterationUrl);
        }

        [Fact]
        public void BriefDescription_ShouldHaveValidationError_WhenExceedsMaxLength()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                BriefDescription = new string('a', 34),
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.BriefDescription)
                .WithErrorMessage("The Brief Description cannot be longer than 33 characters");
        }

        [Fact]
        public void Images_ShouldHaveValidationError_WhenMoreThanOneGif()
        {
            var command = new UpdateStreetcodeMainPageCommand(
                new StreetcodeMainPageUpdateDto
                {
                    Images = new List<ImageFileBaseCreateDto>
                    {
                        new ImageFileBaseCreateDto { MimeType = "gif" },
                        new ImageFileBaseCreateDto { MimeType = "gif" }
                    },
                    TransliterationUrl = ""
                }
            );

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.Images)
                .WithErrorMessage("There should be only one 'gif' image");
        }

        [Fact]
        public void Images_ShouldHaveValidationError_WhenMoreThanTwoImages()
        {
            var command = new UpdateStreetcodeMainPageCommand(
                new StreetcodeMainPageUpdateDto
                {
                    Images = new List<ImageFileBaseCreateDto>
                    {
                        new ImageFileBaseCreateDto { MimeType = "jpg" },
                        new ImageFileBaseCreateDto { MimeType = "png" },
                        new ImageFileBaseCreateDto { MimeType = "gif" }
                    },
                    TransliterationUrl = ""
                }
            );

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.Images)
                .WithErrorMessage("There should be no more than one of each image");
        }

        [Fact]
        public void Images_ShouldNotHaveValidationError_WhenValidConditionsAreMet()
        {
            var command = new UpdateStreetcodeMainPageCommand(
                new StreetcodeMainPageUpdateDto
                {
                    Images = new List<ImageFileBaseCreateDto>
                    {
                        new ImageFileBaseCreateDto { MimeType = "gif" },
                        new ImageFileBaseCreateDto { MimeType = "jpg" }
                    },
                    TransliterationUrl = ""
                }
            );

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.Images);
        }

        [Fact]
        public void Title_ShouldBeValid_WhenLengthIsWithinLimit()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                Title = "Valid Title",
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.Title);
        }

        [Fact]
        public void FirstName_ShouldBeValid_WhenLengthIsWithinLimit()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                FirstName = "ValidFirstName",
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.FirstName);
        }

        [Fact]
        public void LastName_ShouldBeValid_WhenLengthIsWithinLimit()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                LastName = "ValidLastName",
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.LastName);
        }

        [Fact]
        public void Teaser_ShouldBeValid_WhenLengthIsWithinLimit()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                Teaser = "Valid teaser within 450 characters.",
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.Teaser);
        }

        [Fact]
        public void EventStartOrPersonBirthDate_ShouldBeValid_WhenNotEmpty()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                EventStartOrPersonBirthDate = DateTime.Now,
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.EventStartOrPersonBirthDate);
        }

        [Fact]
        public void BriefDescription_ShouldBeValid_WhenLengthIsWithinLimit()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                BriefDescription = "Brief description",
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.BriefDescription);
        }

        [Fact]
        public void Audio_ShouldBeValid_WhenProvidedWithValidAudio()
        {
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                Audio = new AudioFileBaseCreateDto { Title = "audio", MimeType = "mp3" },
                Images = imageListEmpty,
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.Audio);
        }
    }
}
