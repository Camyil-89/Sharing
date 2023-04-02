using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sharing.API;
using Sharing.Http.Server.Services;
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
			Console.WriteLine("ASDASD");
			return new FileStreamResult(new MemoryStream(Encoding.UTF8.GetBytes("Hello")), "application/octet-stream")
			{
				FileDownloadName = "MyData.xml"
			};
		}
	}
}
