using Microsoft.AspNetCore.Mvc;
using Sharing.Http.Server.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sharing.Http.Server.Controllers
{
	[ApiController]
	public class MainController : ControllerBase
	{

		[Route("/ping")]
		[HttpGet]
		public async Task<DateTime> Ping()
		{
			return Settings.LastUpdateSharingFiles;
		}

	}
}
