using Streetcode.BLL.DTO.AdditionalContent;

namespace Streetcode.BLL.DTO.Streetcode.RelatedFigure
{
    public class RelatedFigureDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string? Alias { get; set; }
        public int ImageId { get; set; }
        public IEnumerable<TagDto> Tags { get; set; }
    }
}
