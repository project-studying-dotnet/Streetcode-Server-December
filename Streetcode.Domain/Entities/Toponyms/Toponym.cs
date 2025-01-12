using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.Domain.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.Domain.Entities.Streetcode;

namespace Streetcode.Domain.Entities.Toponyms
{
    [Table("toponyms", Schema = "toponyms")]
    public class Toponym
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Oblast { get; set; }

        [MaxLength(150)]
        public string? AdminRegionOld { get; set; }

        [MaxLength(150)]
        public string? AdminRegionNew { get; set; }

        [MaxLength(150)]
        public string? Gromada { get; set; }

        [MaxLength(150)]
        public string? Community { get; set; }

        [Required]
        [MaxLength(150)]
        public string StreetName { get; set; }

        [MaxLength(50)]
        public string? StreetType { get; set; }

        public List<StreetcodeContent> Streetcodes { get; set; } = new();

        public ToponymCoordinate Coordinate { get; set; }
    }
}