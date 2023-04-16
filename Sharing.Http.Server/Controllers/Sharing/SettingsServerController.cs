using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sharing.API;
using Sharing.API.Models;
using Sharing.Http.Server.Services;

namespace Sharing.Http.Server.Controllers.Sharing
{
	[ApiController]
	public class SettingsServerController : ControllerBase
	{
		[Route("api/settings")]
		[HttpGet]
		public async Task<SettingsServer> GetSettings()
		{
			return Settings.Server;
		}
	}
}
