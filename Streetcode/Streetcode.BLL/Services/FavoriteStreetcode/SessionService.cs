using Microsoft.AspNetCore.Http;
using Streetcode.BLL.Interfaces.FavoriteStreetcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Services.FavoriteStreetcode
{
	public class SessionService : ISessionService
	{
		private string GetKey(int userId) => $"FavoriteStreetcodes_{userId}";

		public async Task AddFavoriteStreetcode(ISession session, string userId, int streetcodeId)
		{
			var favorites = await GetFavoriteStreetcodes(session, userId) ?? new List<int>();
			if (!favorites.Contains(streetcodeId))
			{
				favorites.Add(streetcodeId);
				await session.Set(userId, favorites);
			}
		}

		public async Task<List<int>> GetFavoriteStreetcodes(ISession session, string userId)
		{
			return await session.Get<List<int>>(userId);
		}

		public async Task RemoveFavoriteStreetcode(ISession session, string userId, int streetcodeId)
		{
			var favorites = await GetFavoriteStreetcodes(session, userId);
			if (favorites != null && favorites.Contains(streetcodeId))
			{
				favorites.Remove(streetcodeId);
				await session.Set(userId, favorites);
			}
		}
	}
}
