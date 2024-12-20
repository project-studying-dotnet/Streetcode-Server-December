using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Create
{
    public class CreateTextCommandValidator : AbstractValidator<CreateTextCommand>
    {
        private const int TitleMaxLength = 300;
        private const int TextContentMaxLength = 15000;
        private const int AdditionalTextMaxLength = 200;

        public CreateTextCommandValidator()
        {
            RuleFor(x => x.TextCreateDto.Title)
             .NotEmpty()
             .WithMessage("Text title is required")
             .MaximumLength(TitleMaxLength)
             .WithMessage($"Text title must not exceed {TitleMaxLength} chars");

            RuleFor(x => x.TextCreateDto.TextContent)
                .NotEmpty()
                .WithMessage("Text content is required")
                .MaximumLength(TextContentMaxLength)
                .WithMessage($"Text content must not exceed {TextContentMaxLength} chars");

            RuleFor(x => x.TextCreateDto.AdditionalText)
                .NotEmpty()
                .WithMessage("Text additional text is required")
                .MaximumLength(AdditionalTextMaxLength)
                .WithMessage($"Text additional text must not exceed {AdditionalTextMaxLength} chars");

            RuleFor(x => x.TextCreateDto.StreetcodeId)
                .GreaterThan(0)
                .WithMessage("Text StreetcodeId must be > 0");
        }
    }
}
