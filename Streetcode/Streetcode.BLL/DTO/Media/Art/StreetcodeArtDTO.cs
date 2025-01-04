using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.DTO.Media.Art
{
    public class StreetcodeArtDto
    {
        public int Index { get; set; }
        public int StreetcodeId { get; set; }
        public ArtDto? Art { get; set; }
        public StreetcodeDto? Streetcode { get; set; }
    }
}
