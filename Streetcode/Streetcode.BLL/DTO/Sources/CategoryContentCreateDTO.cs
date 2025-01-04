namespace Streetcode.BLL.DTO.Sources
{
  public class CategoryContentCreateDto
  {
    public int? Id { get; set; }
    public int SourceLinkCategoryId { get; set; }
    public string? Text { get; set; }
    public string? Title { get; set; }
    public int StreetcodeId { get; set; }
  }
}
