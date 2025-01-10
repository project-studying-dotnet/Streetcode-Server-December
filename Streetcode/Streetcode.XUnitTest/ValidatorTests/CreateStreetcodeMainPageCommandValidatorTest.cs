using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.CreateMainPage;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using System.Collections.Generic;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreateStreetcodeMainPageCommandValidatorTest
    {
        private readonly CreateStreetcodeMainPageCommandValidator _validator;
        private readonly List<ImageFileBaseCreateDto> imageListEmpty = new List<ImageFileBaseCreateDto> { };

        public CreateStreetcodeMainPageCommandValidatorTest()
        {
            _validator = new CreateStreetcodeMainPageCommandValidator();
        }

        [Fact]
        public void Validator_Should_HaveError_When_DateStringIsEmpty()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                DateString = "",
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.DateString)
                .WithErrorMessage("The 'Date String' field is required");
        }

        [Fact]
        public void Validator_Should_HaveError_When_DateStringIsLongerThan50Characters()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                DateString = "hcgyux4r,gfehmdgtw9gkuefhldgtiu589t;gietfuldioeufkhlkyuhfe89t8hkul9ed8thuk9658hkul9ehtld",
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.DateString)
                .WithErrorMessage("The Date String cannot be longer than 50 characters");
        }

        [Fact]
        public void DateString_ShouldBeValid_WhenNotEmptyAndLessThan50Characters()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                DateString = "hcgyux4r,gfehmdgtw9gkuefhld",
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.DateString);
        }

        [Fact]
        public void Index_ShouldBeValid_WhenNotEmptyAndGreaterThanOrEqualToZero()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                Index = 1,
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.Index);
        }

        [Fact]
        public void Index_ShouldHaveValidationError_WhenEmptyOrNegative()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                Index = -1,
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.Index)
                .WithErrorMessage("Index must be greater than or equal to 0");
        }

        [Fact]
        public void Title_ShouldHaveValidationError_WhenExceedsMaxLength()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                Title = new string('a', 101),
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.Title)
                .WithErrorMessage("The Title cannot be longer than 100 characters");
        }

        [Fact]
        public void FirstName_ShouldHaveValidationError_WhenExceedsMaxLength()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                FirstName = new string('a', 51),
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.FirstName)
                .WithErrorMessage("The First Name cannot be longer than 50 characters");
        }

        [Fact]
        public void LastName_ShouldHaveValidationError_WhenExceedsMaxLength()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                LastName = new string('a', 51),
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.LastName)
                .WithErrorMessage("The Last Name cannot be longer than 50 characters");
        }

        [Fact]
        public void Teaser_ShouldHaveValidationError_WhenExceedsMaxLength()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                Teaser = new string('a', 451),
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.Teaser)
                .WithErrorMessage("The Teaser cannot be longer than 450 characters");
        }

        [Fact]
        public void EventStartOrPersonBirthDate_ShouldHaveValidationError_WhenEmpty()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.EventStartOrPersonBirthDate)
                .WithErrorMessage("The 'Event Start Or Person Birth Date' field is required");
        }

        [Fact]
        public void TransliterationUrl_ShouldHaveValidationError_WhenInvalid()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                TransliterationUrl = "Invalid_Url",
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.TransliterationUrl)
                .WithErrorMessage("Transliteration Url is allowed to accept lowercase latin letters, numbers, and hyphens.");
        }

        [Fact]
        public void TransliterationUrl_ShouldNotHaveValidationError_WhenValid()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                TransliterationUrl = "valid-url-123",
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.TransliterationUrl);
        }

        [Fact]
        public void BriefDescription_ShouldHaveValidationError_WhenExceedsMaxLength()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                BriefDescription = new string('a', 34),
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeMainPage.BriefDescription)
                .WithErrorMessage("The Brief Description cannot be longer than 33 characters");
        }

        [Fact]
        public void Images_ShouldHaveValidationError_WhenMoreThanOneGif()
        {
            var command = new CreateStreetcodeMainPageCommand(
                new StreetcodeMainPageCreateDto
                {
                    Images = new List<ImageFileBaseCreateDto>
                    {
                        new ImageFileBaseCreateDto { MimeType = "gif" },
                        new ImageFileBaseCreateDto { MimeType = "gif" }
                    },
                    Tags = Array.Empty<StreetcodeTagDto>(),
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
            var command = new CreateStreetcodeMainPageCommand(
                new StreetcodeMainPageCreateDto
                {
                    Images = new List<ImageFileBaseCreateDto>
                    {
                        new ImageFileBaseCreateDto { MimeType = "jpg" },
                        new ImageFileBaseCreateDto { MimeType = "png" },
                        new ImageFileBaseCreateDto { MimeType = "gif" }
                    },
                    Tags = Array.Empty<StreetcodeTagDto>(),
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
            var command = new CreateStreetcodeMainPageCommand(
                new StreetcodeMainPageCreateDto
                {
                    Images = new List<ImageFileBaseCreateDto>
                    {
                        new ImageFileBaseCreateDto { MimeType = "gif" },
                        new ImageFileBaseCreateDto { MimeType = "jpg" }
                    },
                    Tags = Array.Empty<StreetcodeTagDto>(),
                    TransliterationUrl = ""
                }
            );

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.Images);
        }

        [Fact]
        public void Title_ShouldBeValid_WhenLengthIsWithinLimit()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                Title = "Valid Title",
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.Title);
        }

        [Fact]
        public void FirstName_ShouldBeValid_WhenLengthIsWithinLimit()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                FirstName = "ValidFirstName",
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.FirstName);
        }

        [Fact]
        public void LastName_ShouldBeValid_WhenLengthIsWithinLimit()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                LastName = "ValidLastName",
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.LastName);
        }

        [Fact]
        public void Teaser_ShouldBeValid_WhenLengthIsWithinLimit()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                Teaser = "Valid teaser within 450 characters.",
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.Teaser);
        }

        [Fact]
        public void EventStartOrPersonBirthDate_ShouldBeValid_WhenNotEmpty()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                EventStartOrPersonBirthDate = DateTime.Now,
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.EventStartOrPersonBirthDate);
        }

        [Fact]
        public void BriefDescription_ShouldBeValid_WhenLengthIsWithinLimit()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                BriefDescription = "Brief description",
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.BriefDescription);
        }

        [Fact]
        public void Audio_ShouldBeValid_WhenProvidedWithValidAudio()
        {
            var command = new CreateStreetcodeMainPageCommand(new StreetcodeMainPageCreateDto
            {
                Audio = new AudioFileBaseCreateDto { Title = "audio", MimeType = "mp3" },
                Images = imageListEmpty,
                Tags = Array.Empty<StreetcodeTagDto>(),
                TransliterationUrl = ""
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeMainPage.Audio);
        }
    }
}
