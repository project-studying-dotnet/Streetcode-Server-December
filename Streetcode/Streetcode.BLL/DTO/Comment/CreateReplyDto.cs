using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Comment
{
    public class CreateReplyDto
    {
        public string UserFullName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? DateModified { get; set; }
        public string Content { get; set; } = string.Empty;
        public int StreetcodeId { get; set; }
        public int ParentId { get; set; }
    }
}
