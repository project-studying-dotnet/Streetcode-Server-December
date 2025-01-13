using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BLL.Interfaces.User
{
    public interface ITokenCleanupService 
    {
        public Task RemoveExpiredRefreshTokensAsync();
    }
}
