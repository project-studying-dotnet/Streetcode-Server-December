using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.Domain.Entities.Streetcode;

namespace Streetcode.Domain.Entities.Media.Images
{
    [Table("arts", Schema = "media")]
    public class Art
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(400)]
        public string? Description { get; set; }

        [MaxLength(150)]
        public string? Title { get; set; }

        public int ImageId { get; set; }

        public Image? Image { get; set; }

        public List<StreetcodeArt> StreetcodeArts { get; set; } = new();
    }
}