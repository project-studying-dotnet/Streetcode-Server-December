using System.ComponentModel.DataAnnotations;
using Streetcode.Domain.Entities.Analytics;
using Streetcode.Domain.Entities.Streetcode;

namespace Streetcode.Domain.Entities.AdditionalContent.Coordinates.Types
{
    public class StreetcodeCoordinate : Coordinate
    {
        [Required]
        public int StreetcodeId { get; set; }

        public StreetcodeContent? Streetcode { get; set; }

        public StatisticRecord StatisticRecord { get; set; }
    }
}