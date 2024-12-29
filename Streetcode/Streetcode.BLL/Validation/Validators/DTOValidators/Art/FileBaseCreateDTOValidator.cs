using FluentValidation;
using Microsoft.VisualBasic.FileIO;
using Streetcode.BLL.DTO.Media;
namespace Streetcode.BLL.Validation.Validators.DTOValidators.Art
{
    public class FileBaseCreateDTOValidator : AbstractValidator<FileBaseCreateDto>
    {
        private static readonly HashSet<string> _imageMimeTypes = new()
        {
            "jpeg",
            "png",
            "gif",
            "bmp",
            "webp",
            "svg"
        };

        private static readonly HashSet<string> _audioMimeTypes = new()
        {
            "mp3",
            "wav",
            "ogg",
            "aac",
            "flac"
        };

        private static readonly HashSet<string> _videoMimeTypes = new()
        {
            "mp4",
            "avi",
            "mkv",
            "mov",
            "flv",
            "webm"
        };

        public FileBaseCreateDTOValidator()
        {
        }

        public FileBaseCreateDTOValidator(string fileType)
        {
            var allowedMimeTypes = fileType switch
            {
                "image" => _imageMimeTypes,
                "audio" => _audioMimeTypes,
                "video" => _videoMimeTypes,
                _ => throw new ArgumentException("Unsupported file type")
            };

            var mimeTypeMessage = fileType switch
             {
                 "image" => string.Join(",", _imageMimeTypes.Select(i => i)),
                 "audio" => string.Join(",", _audioMimeTypes.Select(i => i)),
                 "video" => string.Join(",", _videoMimeTypes.Select(i => i)),
                 _ => throw new ArgumentException("Unsupported file type")
             };

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty")
                .Length(2, 200).WithMessage("Title must be between 2 and 200 characters");

            RuleFor(x => x.BaseFormat)
                .NotEmpty().WithMessage("BaseFormat cannot be empty.")
                .Matches("^[A-Za-z0-9]+$").WithMessage("BaseFormat can only contain letters and numbers");

            RuleFor(x => x.MimeType)
                .NotEmpty().WithMessage("MimeType cannot be empty.")
                .Must(mimeType => allowedMimeTypes.Contains(mimeType.ToLower()))
                .WithMessage(mimeTypeMessage);

            RuleFor(x => x.Extension)
                .NotEmpty().WithMessage("Extension cannot be empty.")
                .Matches(@"^\.[a-zA-Z0-9]+$").WithMessage("Extension must start with a dot and contain valid characters (.jpg)");
        }
    }
}
