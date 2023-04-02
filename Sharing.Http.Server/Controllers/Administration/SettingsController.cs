using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sharing.API.Models;
using Sharing.Http.Server.Services;

namespace Sharing.Http.Server.Controllers.Administration
{
	
	[ApiController]
	public class SettingsController : ControllerBase
	{
		[HttpPost]
		[Route("api/[controller]/set_sharing")]
		public async Task<IActionResult> SetSharing()
		{
			if (HttpContext.Connection.RemoteIpAddress.MapToIPv4().Address != 16777343)
				return StatusCode(1020);
			Settings.SharingFiles = await HttpContext.Request.ReadFromJsonAsync<List<API.Models.SharingFile>>();
			Settings.LastUpdateSharingFiles = DateTime.Now;
			return Ok();
		}
	}
}
