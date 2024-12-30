namespace Streetcode.BLL.DTO.Media.Art
{
    public class ArtCreateDto
    {
        public string? Description { get; set; }
        public string? Title { get; set; }
        public int ImageId { get; set; }
        public List<int>? StreetcodeIds { get; set; }
    }
}
