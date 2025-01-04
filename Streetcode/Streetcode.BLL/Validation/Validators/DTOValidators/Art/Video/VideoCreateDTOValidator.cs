using FluentValidation;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Media.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Art.Video
{
    public class VideoCreateDTOValidator : AbstractValidator<VideoCreateDto>
    {
        public VideoCreateDTOValidator()
        {
            RuleFor(x => x.StreetcodeId)
               .GreaterThan(0)
               .WithMessage("StreetcodeId must be greater than 0"); 

            RuleFor(x => x.Url)
               .Matches(@"^(http|https)://").When(x => !string.IsNullOrEmpty(x.Url))
               .WithMessage("Url must be a valid URL starting with http:// or https://");
        }
    }
}
