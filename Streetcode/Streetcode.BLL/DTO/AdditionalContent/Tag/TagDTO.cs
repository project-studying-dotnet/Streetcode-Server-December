using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.DTO.AdditionalContent
{
    public class TagDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public IEnumerable<StreetcodeDto>? Streetcodes { get; set; }
    }
}
