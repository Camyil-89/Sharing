using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sharing.Http.Server.Controllers.Sharing
{
	[ApiController]
	public class FilesController : Controller
	{
		[Route("api/sharing/files")]
		[HttpGet]
		public string Get()
		{
			return "test";
		}
	}
}
