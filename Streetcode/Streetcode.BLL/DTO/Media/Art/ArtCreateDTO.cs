namespace Streetcode.BLL.DTO.Media.Art
{
    public class ArtCreateDTO
    {
        public string? Description { get; set; }
        public string? Title { get; set; }
        public int ImageId { get; set; }
        public List<int>? StreetcodeIds { get; set; }
    }
}
