using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.DTO.Toponyms
{
    public class ToponymDto
    {
        public int Id { get; set; }
        public string Oblast { get; set; }
        public string? AdminRegionOld { get; set; }
        public string? AdminRegionNew { get; set; }
        public string? Gromada { get; set; }
        public string? Community { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }

        public ToponymCoordinateDto Coordinate { get; set; }
        public IEnumerable<StreetcodeDto> Streetcodes { get; set; }
    }
}