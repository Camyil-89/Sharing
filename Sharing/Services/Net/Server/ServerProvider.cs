using Microsoft.Extensions.DependencyInjection;
using Sharing.ViewModels.Pages.SharingVM;
using Sharing.ViewModels.Windows.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Sharing.Services.Net.Server
{
	internal static class ServerProvider
	{
		private static Sharing.Server.Server Server;
		private static NetFind.Server NetFindServer = new NetFind.Server();
		private static ViewModels.Pages.SharingVM.SharingPageVM SharingPageVM = App.Host.Services.GetRequiredService<ViewModels.Pages.SharingVM.SharingPageVM>();
		private static ViewModels.Windows.Main.MainWindowVM MainWindowVM = App.Host.Services.GetRequiredService<MainWindowVM>();

		public static void Init()
		{
			MainWindowVM.IPaddressServer = "127.0.0.1";
		}
		private static void Loop()
		{
			Log.WriteLine("[Server] [Loop] start", LogLevel.Warning);
			Stopwatch update_list_clients = Stopwatch.StartNew();
			while (Server != null)
			{
				try
				{
					if (update_list_clients.ElapsedMilliseconds >= 1000)
					{
						SharingPageVM.Clients.Clear();

						foreach (var i in Server.Clients)
						{
							ItemClient client = new ItemClient();
							client.Port = i.PortClient;
							client.IPaddress = i.IPClient;
							client.Ping = i.Ping;
							client.TimeConnect = $"{(DateTime.Now - i.TimeConnect).Hours}:{(DateTime.Now - i.TimeConnect).Minutes}:{(DateTime.Now - i.TimeConnect).Seconds}";
							SharingPageVM.Clients.Add(client);
						}
						update_list_clients.Restart();
					}
					if (Settings.Instance.Parametrs.SharingFilesAndFolders.Count != Sharing.Server.Server.SharingFolders.Count)
					{
						Console.WriteLine("asd");
						Server.SetSharingFolder(Settings.Instance.Parametrs.SharingFilesAndFolders.ToList());
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
				Thread.Sleep(100);
			}
			Log.WriteLine("[Server] [Loop] stop", LogLevel.Warning);
		}
		public static void Start(int port)
		{
			if (Server != null)
			{
				Server.Stop();
				Server = new Sharing.Server.Server();
			}
			Server = new Sharing.Server.Server();
			Server.Start(port);
			//NetFindServer = new NetFind.Server();
			//Task.Run(() => { NetFindServer.Start(Settings.Instance.Parametrs.NetFindServerPort, "Sharing", port); });
			Task.Run(Loop);
			MainWindowVM.VisibilityServerStatus = System.Windows.Visibility.Visible;
			SharingPageVM.StartStopButtonText = "Остановить";
		}
		public static void Stop()
		{
			NetFindServer.Stop();
			Server.Stop();
			Server = null;
			SharingPageVM.StartStopButtonText = "Запустить";
			MainWindowVM.VisibilityServerStatus = System.Windows.Visibility.Collapsed;
			SharingPageVM.Clients.Clear();
		}

		public static Sharing.Server.StatusServer GetStatusServer()
		{
			if (Server != null)
				return Server.Status;
			return Sharing.Server.StatusServer.Stop;
		}
	}
}
