using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Streetcode
{
    public class StreetcodeMainPageCreateDto
    {
        public int Index { get; set; }
        public StreetcodeType StreetcodeType { get; set; }
        public string? Title { get; set; }
        public string? DateString { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Teaser { get; set; }
        public DateTime EventStartOrPersonBirthDate { get; set; }
        public DateTime? EventEndOrPersonDeathDate { get; set; }
        public IEnumerable<StreetcodeTagDto> Tags { get; set; }
        public string TransliterationUrl { get; set; }
        public string? BriefDescription { get; set; }
        public List<ImageFileBaseCreateDto> Images { get; set; }
        public AudioFileBaseCreateDto? Audio { get; set; }
    }
}
