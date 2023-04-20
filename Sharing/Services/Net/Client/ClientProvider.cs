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
using System.Runtime.Loader;
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
			DowloadPageVM.VisibilityResumeBtn = File.Exists(Settings.Instance.Parametrs.LastDowload) ? System.Windows.Visibility.Visible: System.Windows.Visibility.Collapsed;
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
				Thread.Sleep(16);
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
			MainWindowVM.IPaddressConnectServer = $"{address.ToString()}";
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
			Task.Run(InternalLoop);
			MainWindowVM.VisibilityClientStatus = System.Windows.Visibility.Visible;
			DowloadPageVM.IsEnableSettings = false;
			DowloadPageVM.StartStopButtonText = "Отключиться";
		}
		public static void Stop()
		{
			HttpClient.Stop();

			MainWindowVM.VisibilityClientStatus = System.Windows.Visibility.Collapsed;
			DowloadPageVM.ListNodes.Clear();
			DowloadPageVM.StartStopButtonText = "Подключиться";
			DowloadPageVM.IsEnableSettings = true;
		}

		public static Sharing.Http.Client.Status GetStatus()
		{
			if (HttpClient == null)
				return Http.Client.Status.Shutdown;
			return HttpClient.Status;
		}

		public static SettingsServer GetSettingsServer()
		{
			return HttpClient.Requests.GetSettingsServer();
		}

		public static void SetResumePathDowload(string path)
		{
			Settings.Instance.Parametrs.LastDowload = path;
			DowloadPageVM.VisibilityResumeBtn = File.Exists(Settings.Instance.Parametrs.LastDowload) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			Program.SaveSettings();
		}

		public static void DowloadFile(DowloadInfo dowloadInfo)
		{
			dowloadInfo.StartDowload(HttpClient.Requests);
		}
	}
}
