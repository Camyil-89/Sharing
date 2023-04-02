using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sharing.API;
using Sharing.Http.Server.Services;

namespace Sharing.Http.Server.Controllers.Sharing
{
	[ApiController]
	public class FilesController : Controller
	{
		[Route("api/sharing/files")]
		[HttpGet]
		public async Task<List<ItemTree>> GetFiles()
		{
			return Utilities.CreateItemTree(Settings.SharingFiles);
		}
	}
}
