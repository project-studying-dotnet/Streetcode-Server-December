using Streetcode.BLL.DTO.Media;
using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.Fact
{
    public class CreateFactImageDTO : ImageFileBaseCreateDTO
    {
        public CreateFactImageDetailsDTO? ImageDetails { get; set; }
    }
}
