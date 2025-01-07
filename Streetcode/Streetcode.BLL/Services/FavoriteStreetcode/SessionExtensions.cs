using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Streetcode.BLL.Services.FavoriteStreetcode
{
	public static class SessionExtensions
	{
		public static async Task Set<T>(this ISession session, string key, T value)
		{
			var json = JsonSerializer.Serialize(value);
			session.SetString(key, json);
		}

		public static async Task<T> Get<T>(this ISession session, string key)
		{
			var json = session.GetString(key);
			return json == null ? default : JsonSerializer.Deserialize<T>(json);
		}
	}
}
