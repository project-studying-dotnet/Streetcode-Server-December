using Streetcode.DAL.Enums;

namespace Streetcode.BLL.DTO.Timeline.Create
{
    public class TimelineItemCreateDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public DateViewPattern DateViewPattern { get; set; }
        public IEnumerable<HistoricalContextDTO>? HistoricalContexts { get; set; } = new List<HistoricalContextDTO>();
        public int StreetcodeId { get; set; }
        public string Context { get; set; }
    }
}
