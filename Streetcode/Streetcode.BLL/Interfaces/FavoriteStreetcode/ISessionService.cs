using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Interfaces.FavoriteStreetcode
{
	public interface ISessionService
	{
		Task AddFavoriteStreetcode(ISession session, string userId, int streetcodeId);
		Task<List<int>> GetFavoriteStreetcodes(ISession session, string userId);
		Task RemoveFavoriteStreetcode(ISession session, string userId, int streetcodeId);
	}
}
