using Microsoft.AspNetCore.Mvc;
using Polly;
using Streetcode.BLL.Interfaces.FavoriteStreetcode;
using System.Security.Claims;

namespace Streetcode.WebApi.Controllers.FavoriteStreetcode
{
	[ApiController]
	[Route("api/[controller]")]
	public class FavoriteStreetcodeController : ControllerBase
	{
		private readonly ISessionService _sessionService;
		private readonly IHttpContextAccessor _contextAccessor;
		

		public FavoriteStreetcodeController(
			ISessionService sessionService,
			IHttpContextAccessor contextAccessor)
		{
			_sessionService = sessionService;
			_contextAccessor = contextAccessor;
			
		}

		[HttpPost]
		public async Task<IActionResult> AddFavorite(int streetcodeId)
		{
			try
			{
				var user = _contextAccessor.HttpContext.User;
				var userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User not authenticated.");
				}

				var session = _contextAccessor.HttpContext?.Session;
				if (session == null)
				{
					return StatusCode(StatusCodes.Status500InternalServerError, "Session is not available.");
				}

				await _sessionService.AddFavoriteStreetcode(session, userId, streetcodeId);
				return Ok(new { message = "Streetcode added to favorites." });
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding favorite: {ex.Message}");
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetFavorites()
		{
			try
			{
				var user = _contextAccessor.HttpContext.User;
				var userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User not authenticated.");
				}

				var session = _contextAccessor.HttpContext?.Session;
				if (session == null)
				{
					return StatusCode(StatusCodes.Status500InternalServerError, "Session is not available.");
				}

				var streetcodes = await _sessionService.GetFavoriteStreetcodes(session, userId);
				return Ok(streetcodes ?? new List<int>());
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving favorites: {ex.Message}");
			}
		}

		[HttpDelete]
		public async Task<IActionResult> RemoveFavorite(int streetcodeId)
		{
			try
			{
				var user = _contextAccessor.HttpContext.User;
				var userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User not authenticated.");
				}

				var session = _contextAccessor.HttpContext?.Session;
				if (session == null)
				{
					return StatusCode(StatusCodes.Status500InternalServerError, "Session is not available.");
				}

				await _sessionService.RemoveFavoriteStreetcode(session, userId, streetcodeId);
				return Ok(new { message = "Streetcode removed from favorites." });
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error removing favorite: {ex.Message}");
			}
		}
	}
}
