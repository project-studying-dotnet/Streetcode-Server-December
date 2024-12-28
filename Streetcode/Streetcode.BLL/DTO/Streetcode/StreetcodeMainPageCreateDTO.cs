using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Streetcode
{
    public class StreetcodeMainPageCreateDTO
    {
        public int Index { get; set; }
        public StreetcodeType StreetcodeType { get; set; }
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Teaser { get; set; }
        public DateTime EventStartOrPersonBirthDate { get; set; }
        public DateTime? EventEndOrPersonDeathDate { get; set; }
        public IEnumerable<StreetcodeTagDTO> Tags { get; set; }
        public string TransliterationUrl { get; set; }
        public string? BriefDescription { get; set; }
        public List<ImageFileBaseCreateDTO> Images { get; set; }
        public AudioFileBaseCreateDTO? Audio { get; set; }
    }
}
