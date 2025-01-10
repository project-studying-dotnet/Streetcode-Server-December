using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.DTO.Streetcode
{
    public class StreetcodeDto
    {
        public int Id { get; set; }
        public int Index { get; set; }
        required public string Title { get; set; }
        required public string DateString { get; set; }
        public string? Alias { get; set; }
        required public string TransliterationUrl { get; set; }
        public StreetcodeStatus Status { get; set; }
        public DateTime EventStartOrPersonBirthDate { get; set; }
        public DateTime? EventEndOrPersonDeathDate { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        required public IEnumerable<StreetcodeTagDto> Tags { get; set; }
        public string? Teaser { get; set; }
        public StreetcodeType StreetcodeType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
