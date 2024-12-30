using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.DTO.Sources
{
    public class SourceLinkCategoryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ImageId { get; set; }
        public ImageDto? Image { get; set; }
    }
}