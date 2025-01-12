using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.Domain.Entities.Media.Images;

namespace Streetcode.Domain.Entities.News
{
    [Table("news", Schema = "news")]
    [Index(nameof(URL), IsUnique = true)]
    public class News
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        [MaxLength(100)]
        public string URL { get; set; }
        public int? ImageId { get; set; }
        public Image? Image { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
    }
}
