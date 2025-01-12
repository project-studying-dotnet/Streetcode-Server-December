﻿using Streetcode.Domain.Enums;

namespace Streetcode.BLL.DTO.Timeline.Update
{
    public class TimelineItemUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public DateViewPattern DateViewPattern { get; set; }
        public IEnumerable<HistoricalContextDto>? HistoricalContexts { get; set; } = new List<HistoricalContextDto>();
        public int StreetcodeId { get; set; }
    }
}
