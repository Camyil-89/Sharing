using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sharing.API;
using Sharing.Http.Server.Services;
using Sharing.Services;
using System.IO;
using System.Net;
using System.Text;

namespace Sharing.Http.Server.Controllers.Sharing
{
	[ApiController]
	public class DowloadController : ControllerBase
	{
		[Route("api/[controller]/files")]
		[HttpGet]
		public async Task<FileStreamResult> Dowload()
		{
			try
			{
				var file = await HttpContext.Request.ReadFromJsonAsync<API.Models.RequestFileInfo>();
				var path = Utilities.GetSharingFilePath(Settings.SharingFiles, file);
				if (!System.IO.File.Exists(path))
					return new FileStreamResult(new MemoryStream(), "application/octet-stream");
				return new FileStreamResult(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096), "application/octet-stream");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return new FileStreamResult(new MemoryStream(), "application/octet-stream");
			}

		}
	}
}
