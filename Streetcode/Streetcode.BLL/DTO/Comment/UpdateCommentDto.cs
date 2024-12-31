using System;
namespace Streetcode.BLL.DTO.Comment
{
    public class UpdateCommentDto
    {
        public string UserFullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DateModified { get; set; }
        public string Content { get; set; } = string.Empty;
        public int StreetcodeId { get; set; }
    }
}
