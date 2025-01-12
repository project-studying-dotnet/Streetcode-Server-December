using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.Domain.Entities.Streetcode;
using Streetcode.Domain.Enums;

namespace Streetcode.Domain.Entities.Timeline
{
    [Table("timeline_items", Schema = "timeline")]
    public class TimelineItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public DateViewPattern DateViewPattern { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(600)]
        public string? Description { get; set; }

        public int StreetcodeId { get; set; }

        public StreetcodeContent? Streetcode { get; set; }

        public List<HistoricalContextTimeline> HistoricalContextTimelines { get; set; } = new();
    }
}
