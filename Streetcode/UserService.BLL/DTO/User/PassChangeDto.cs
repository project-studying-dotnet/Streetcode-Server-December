using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BLL.DTO.User
{
    public class PassChangeDto
    {
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string OldPassword { get; set; }
    }
}
