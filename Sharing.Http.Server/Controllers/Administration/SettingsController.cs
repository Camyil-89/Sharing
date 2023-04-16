using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sharing.API.Models;
using Sharing.Http.Server.Services;
using System.Text;

namespace Sharing.Http.Server.Controllers.Administration
{

	[ApiController]
	public class SettingsController : ControllerBase
	{
		[HttpPost]
		[Route("api/[controller]/set_sharing")]
		public async Task<IActionResult> SetSharing()
		{
			try
			{
				if (HttpContext.Connection.RemoteIpAddress.MapToIPv4().Address != 16777343)
					return StatusCode(1020);
				Settings.SharingFiles = await HttpContext.Request.ReadFromJsonAsync<List<API.Models.SharingFile>>();
				Settings.LastUpdateSharingFiles = DateTime.Now;
				return Ok();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status400BadRequest);
			}
		}

		[HttpPost]
		[Route("api/[controller]/set_settings")]
		public async Task<IActionResult> SetSettings()
		{
			try
			{
				if (HttpContext.Connection.RemoteIpAddress.MapToIPv4().Address != 16777343)
					return StatusCode(1020);
				Settings.Server = await HttpContext.Request.ReadFromJsonAsync<API.Models.SettingsServer>();
				return Ok();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status400BadRequest);
			}
		}
	}
}
