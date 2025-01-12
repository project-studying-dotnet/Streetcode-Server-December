using System.ComponentModel.DataAnnotations;
using Streetcode.Domain.Entities.Toponyms;

namespace Streetcode.Domain.Entities.AdditionalContent.Coordinates.Types
{
    public class ToponymCoordinate : Coordinate
    {
        [Required]
        public int ToponymId { get; set; }

        public Toponym? Toponym { get; set; }
    }
}