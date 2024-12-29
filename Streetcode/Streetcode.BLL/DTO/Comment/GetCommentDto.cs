using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Comment
{
    public class GetCommentDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string UserFullName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? DateModified { get; set; }
        public string Content { get; set; } = null!;
        public int StreetcodeId { get; set; }
        public int? ParentId { get; set; }
        public List<GetCommentDto>? Children { get; set; }
    }
}
