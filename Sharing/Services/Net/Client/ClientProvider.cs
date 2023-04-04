using Microsoft.Extensions.DependencyInjection;
using Sharing.API;
using Sharing.API.Models;
using Sharing.Http.Client;
using Sharing.ViewModels.Pages.Dowload;
using Sharing.ViewModels.Windows.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sharing.Services.Net.Client
{
	internal static class ClientProvider
	{
		private static ViewModels.Pages.Dowload.DowloadPageVM DowloadPageVM = App.Host.Services.GetRequiredService<ViewModels.Pages.Dowload.DowloadPageVM>();
		private static ViewModels.Windows.Main.MainWindowVM MainWindowVM = App.Host.Services.GetRequiredService<MainWindowVM>();

		private static Http.Client.Client HttpClient;


		private static DateTime LastUpdateTree;

		private static int Ping_Timeout = 1000;

		public static void Init()
		{

		}
		private static void InternalLoop()
		{
			Stopwatch stopwatch_ping = Stopwatch.StartNew();
			while (HttpClient.Status == Http.Client.Status.OK)
			{
				try
				{
					if (stopwatch_ping.ElapsedMilliseconds >= Ping_Timeout)
					{
						stopwatch_ping.Restart();
						DateTime time = DateTime.Now;
						var last = HttpClient.Requests.LastUpdateTree();

						if (last != LastUpdateTree)
						{
							UpdateTree();
							LastUpdateTree = last;
						}

						MainWindowVM.TextPing = $"{Math.Round((DateTime.Now - time).TotalMilliseconds, 1)} мс";
					}
				}
				catch (Exception ex)
				{
					Log.WriteLine(ex, LogLevel.Error);
					break;
				}
			}
			Stop();
			MainWindowVM.TextPing = "Нет подключения";
		}
		private static void UpdateTree()
		{
			List<API.ItemTree> itemTrees = HttpClient.Requests.GetTree();
			DowloadPageVM.ListNodes.Clear();
			TreeViewItem generateTreeView(ItemTree item)
			{
				TreeViewItem treeViewItem = new TreeViewItem();
				treeViewItem.Header = item.Name;
				treeViewItem.Tag = item;
				if (item.IsFolder)
				{
					foreach (var i in item.ItemsTrees)
					{
						treeViewItem.Items.Add(generateTreeView(i));
					}
				}
				return treeViewItem;
			}
			foreach (var i in itemTrees)
			{
				App.Current.Dispatcher.Invoke(() =>
				{
					DowloadPageVM.ListNodes.Add(generateTreeView(i));
				});
			}
		}
		public static void Start(IPAddress address, int port)
		{
			LastUpdateTree = DateTime.Now;
			HttpClient = new Http.Client.Client();
			if (!HttpClient.Connect($"http://{address}:{port}"))
			{
				Stop();
				return;
			}
			try
			{
				LastUpdateTree = HttpClient.Requests.LastUpdateTree();
				UpdateTree();
			}
			catch
			{
				Stop();
				return;
			}
			try
			{
				//var di = new DowloadInfo() { Files = new List<RequestFileInfo>() { new API.Models.RequestFileInfo() { Path = "", UID_ROOT = "18E67CF606C6E141B6DD79DA9279FEFDEFE03F4D566C6D629021ADDB16A46944", TotalSize = 24672 } } };
				//var di = new DowloadInfo() { Files = new List<RequestFileInfo>() { new API.Models.RequestFileInfo() { Path = "", UID_ROOT = "C1E440A6CFF901294D802403DCED44A2D6271D9DAF50E49BB73AAC3F4A1D420F", TotalSize = 907 } } };
				var di = new DowloadInfo() { Files = new List<RequestFileInfo>() { new API.Models.RequestFileInfo() { Path = "", UID_ROOT = "CE645D6187176138C451D6209111770942F3D0E01E7B9F00880DD1DC03A3BF2D", TotalSize = 147968 } } };
				di.StartDowload(HttpClient.Requests, $"{Environment.CurrentDirectory}\\test.exe");
			}
			catch (Exception ex) { Console.WriteLine(ex); }
			Task.Run(InternalLoop);
			MainWindowVM.VisibilityClientStatus = System.Windows.Visibility.Visible;
			DowloadPageVM.StartStopButtonText = "Отключиться";
		}
		public static void Stop()
		{
			HttpClient.Stop();

			MainWindowVM.VisibilityClientStatus = System.Windows.Visibility.Collapsed;
			DowloadPageVM.ListNodes.Clear();
			DowloadPageVM.StartStopButtonText = "Подключиться";
		}

		public static Sharing.Http.Client.Status GetStatus()
		{
			if (HttpClient == null)
				return Http.Client.Status.Shutdown;
			return HttpClient.Status;
		}
	}
}
