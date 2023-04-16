using Sharing.API.Models;
using System.Runtime.InteropServices;

namespace Sharing.Http.Server.Services
{
	public static class Settings
	{
		public static List<Sharing.API.Models.SharingFile> SharingFiles = new List<API.Models.SharingFile>();

		public static DateTime LastUpdateSharingFiles = DateTime.Now;

		public static SettingsServer Server = new SettingsServer()
		{
			MaxSizeBlock = 2048,
			SizeBuffer = 1,
		};
	}
}
