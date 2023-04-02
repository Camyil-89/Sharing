namespace Sharing.Http.Server.Services
{
	public static class Settings
	{
		public static List<Sharing.API.Models.SharingFile> SharingFiles = new List<API.Models.SharingFile>();

		public static DateTime LastUpdateSharingFiles = DateTime.Now;
	}
}
