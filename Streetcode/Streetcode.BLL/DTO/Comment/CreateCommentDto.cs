using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.DTO.Comment;

public class CreateCommentDto
{
    public string UserFullName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? DateModified { get; set; }
    public string Content { get; set; } = string.Empty;
    public int StreetcodeId { get; set; }
    public int? ParentId { get; set; } = null;
}