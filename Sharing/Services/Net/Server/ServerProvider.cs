using Microsoft.Extensions.DependencyInjection;
using Sharing.API;
using Sharing.API.Net;
using Sharing.Http.Client;
using Sharing.ViewModels.Pages.SharingVM;
using Sharing.ViewModels.Windows.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Sharing.Services.Net.Server
{
	public	enum StatusServer: byte
	{
		OK = 1,
		Shutdown = 0
	}
	internal static class ServerProvider
	{
		private static ViewModels.Pages.SharingVM.SharingPageVM SharingPageVM = App.Host.Services.GetRequiredService<ViewModels.Pages.SharingVM.SharingPageVM>();
		private static ViewModels.Windows.Main.MainWindowVM MainWindowVM = App.Host.Services.GetRequiredService<MainWindowVM>();

		private static Requests Requests;

		public static void Init()
		{
			var addresses = Utilities.GetLocalIPAddresses();
			MainWindowVM.IPaddressServer = $"{addresses[0].Address}";

			MainWindowVM.TextToolTipAllIPAddresses = "";
			foreach (var i in addresses)
			{
				MainWindowVM.TextToolTipAllIPAddresses += $"{i.Address}\n";
			}
			MainWindowVM.TextToolTipAllIPAddresses = MainWindowVM.TextToolTipAllIPAddresses.Trim();

			//File.WriteAllText($"{Environment.CurrentDirectory}\\secret_password.txt", $"{}");
		}

		public static void Stop()
		{
			ServerProcess.Kill();
			ServerProcess = null;
			MainWindowVM.VisibilityServerStatus = System.Windows.Visibility.Collapsed;
			SharingPageVM.StartStopButtonText = "Запустить";
		}
		static Process ServerProcess = null;
		public static void Start()
		{
			ServerProcess = new Process();
			ServerProcess.StartInfo = new ProcessStartInfo($"{Environment.CurrentDirectory}\\Sharing.Http.Server.exe", $"--urls http://[]:{Settings.Instance.Parametrs.ServerPort}");
			ServerProcess.Start();
			Task.Run(() =>
			{
				ServerProcess.WaitForExit();
				Stop();

			});
			Requests = new Requests($"http://127.0.0.1:{Settings.Instance.Parametrs.ServerPort}");
			SetSharingFolder();
			//Http.Server.Services.Server.Start(new string[] { "--urls", $"http://[::]:{Settings.Instance.Parametrs.ServerPort}" });
			MainWindowVM.VisibilityServerStatus = System.Windows.Visibility.Visible;
			SharingPageVM.StartStopButtonText = "Остановить";
		}

		public static void SetSharingFolder()
		{
			if (GetStatusServer() != StatusServer.OK)
				return;
			try
			{
				Requests.SendPost("/api/settings/set_sharing", Settings.Instance.Parametrs.SharingFilesAndFolders);
			}
			catch (Exception ex)
			{
				Log.WriteLine(ex, LogLevel.Error);
				Stop();
			}
		}

		public static StatusServer GetStatusServer()
		{
			return ServerProcess == null ? StatusServer.Shutdown : StatusServer.OK; //Http.Server.Services.Server.IsStarted ? Sharing.Server.StatusServer.Started : Sharing.Server.StatusServer.Stop;
		}
		//public static Sharing.Server.StatusServer GetStatusServer()
		//{
		//	if (Server != null)
		//		return Server.Status;
		//	return Sharing.Server.StatusServer.Stop;
		//}
	}
}
