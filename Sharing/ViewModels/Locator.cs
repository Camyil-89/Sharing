using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharing.ViewModels
{
	internal class Locator
	{
		public Windows.Main.MainWindowVM MainWindowVM => App.Host.Services.GetRequiredService<Windows.Main.MainWindowVM>();
		public Pages.Dowload.DowloadPageVM DowloadPageVM => App.Host.Services.GetRequiredService<Pages.Dowload.DowloadPageVM>();
		public Pages.Sharing.SharingPageVM SharingPageVM => App.Host.Services.GetRequiredService<Pages.Sharing.SharingPageVM>();


		public Views.Pages.Sharing.SharingPage SharingPage => App.Host.Services.GetRequiredService<Views.Pages.Sharing.SharingPage>();
		public Views.Pages.Settings.SettingsPage SettingsPage => App.Host.Services.GetRequiredService<Views.Pages.Settings.SettingsPage>();
		public Views.Pages.Dowload.DowloadPage DowloadPage => App.Host.Services.GetRequiredService<Views.Pages.Dowload.DowloadPage>();
	}
}
