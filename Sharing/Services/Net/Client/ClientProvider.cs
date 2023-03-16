using Microsoft.Extensions.DependencyInjection;
using Sharing.API;
using Sharing.ViewModels.Pages.Dowload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sharing.Services.Net.Client
{
	internal static class ClientProvider
	{
		private static ViewModels.Pages.Dowload.DowloadPageVM DowloadPageVM = App.Host.Services.GetRequiredService<ViewModels.Pages.Dowload.DowloadPageVM>();
		private static Sharing.Client.Client Client;
		private static NetFind.Client NetFindClient = new NetFind.Client();

		private static bool IsCheckConnectToServer = false;
		private static bool IsCheckConnectToServerNetFind = false;

		public static bool IsActive = false;

		public static void Init()
		{

		}

		private static void HandlerPacketFromServer(Packet packet)
		{
			DowloadPageVM.ListNodes.Clear();
			List<API.ItemTree> itemTrees = (List<API.ItemTree>)packet.Data;
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

		private static void CheckConnectToServer()
		{
			if (IsCheckConnectToServer)
				return;
			Log.WriteLine("[Client] [CheckConnectToServer] strart", LogLevel.Warning);
			IsCheckConnectToServer = true;
			while (Client != null && IsActive)
			{
				try
				{
					if (!Client.Connected() || Client.Status == Sharing.Client.StatusClient.Stop)
					{
						Client.Start(Client.IPAddress, Client.Port);
					}
				}
				catch { }
				Thread.Sleep(16);
			}
			IsCheckConnectToServer = false;
			DowloadPageVM.ActiveStartStopButton = true;
			Log.WriteLine("[Client] [CheckConnectToServer] stop", LogLevel.Warning);
		}

		//private static void CheckConnectToServerNetFind()
		//{
		//	if (IsCheckConnectToServerNetFind)
		//		return;
		//	Log.WriteLine("[Client] [CheckConnectToServerNetFind] strart", LogLevel.Warning);
		//	IsCheckConnectToServerNetFind = true;
		//	while (Client != null && IsActive)
		//	{
		//		try
		//		{
		//			Console.WriteLine(Client.Connected());
		//			if (!Client.Connected() || Client.Status == Sharing.Client.StatusClient.Stop)
		//			{
		//				NetFindClient = new NetFind.Client();
		//				var info = NetFindClient.StartFind(Settings.Instance.Parametrs.ClientNetFindServerPort, 3000, "Sharing");
		//				if (info != null)
		//					Client.Start(info.IPAddress, info.Port);
		//			}
		//		}
		//		catch (Exception ex) { Console.WriteLine(ex); }
		//		Thread.Sleep(16);
		//	}
		//	IsCheckConnectToServerNetFind = false;
		//	DowloadPageVM.ActiveStartStopButton = true;
		//	Log.WriteLine("[Client] [CheckConnectToServerNetFind] stop", LogLevel.Warning);
		//}
		//public static void Start()
		//{
		//	DowloadPageVM.ActiveStartStopButton = false;
		//	if (Client != null)
		//	{
		//		Client.Stop();
		//		Client = null;
		//	}
		//	NetFindClient = new NetFind.Client();
		//  Client = new Sharing.Client.Client();
		//	Client.CallBackPacket += HandlerPacketFromServer;
		//	Task.Run(() =>
		//	{
		//		var info = NetFindClient.StartFind(Settings.Instance.Parametrs.ClientNetFindServerPort, 3000, "Sharing");
		//		if (info != null)
		//			Client.Start(info.IPAddress, info.Port);
		//		DowloadPageVM.ActiveStartStopButton = true;
		//		CheckConnectToServerNetFind();
		//	});
		//	IsActive = true;

		//	DowloadPageVM.StartStopButtonText = "Отключиться";
		//}
		public static void Start(IPAddress address, int port)
		{
			DowloadPageVM.ActiveStartStopButton = false;
			if (Client != null)
			{
				Client.Stop();
				Client = null;
			}
			Client = new Sharing.Client.Client();
			Client.CallBackPacket += HandlerPacketFromServer;
			Client.IPAddress = address;
			Client.Port = port;
			Task.Run(() =>
			{
				Client.Start(Client.IPAddress, Client.Port);
				DowloadPageVM.ActiveStartStopButton = true;
				CheckConnectToServer();
			});
			IsActive = true;

			DowloadPageVM.StartStopButtonText = "Отключиться";
		}
		public static void Stop()
		{
			DowloadPageVM.ActiveStartStopButton = false;
			IsActive = false;

			Task.Run(() =>
			{
				while (IsCheckConnectToServer)
					Thread.Sleep(1);
				Client.Stop();
				Client = null;
			});
			DowloadPageVM.ListNodes.Clear();
			DowloadPageVM.StartStopButtonText = "Подключиться";
		}

		public static Sharing.Client.StatusClient GetStatus()
		{
			if (Client != null)
				return Client.Status;
			return Sharing.Client.StatusClient.Stop;
		}
	}
}
