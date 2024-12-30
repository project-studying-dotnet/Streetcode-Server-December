using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.Fact
{
    public class CreateFactDto
    {
        public string Title { get; set; }
        public string FactContent { get; set; }
        public int StreetcodeId { get; set; }
        public CreateFactImageDto Image { get; set; }
    }
}
