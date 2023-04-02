using Sharing.API;
using Sharing.Services;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Sharing.Server
{
	public enum StatusServer
	{
		Started = 1,
		Stop = 0,
	}
	public class Server
	{
		private TcpListener Listener;

		public StatusServer Status { get; private set; } = StatusServer.Stop;

		public int Ping { get; private set; } = -1;

		public static List<string> SharingFolders = new List<string>();
		public List<Session> Clients = new List<Session>();

		public void SetSharingFolder(List<string> folders)
		{
			SharingFolders = folders;
			while (true)
			{
				//try
				//{
				//	var items_tree = Utilities.CreateItemTree(SharingFolders);
				//	foreach (var i in Clients)
				//	{
				//		i.SendPacket(new Packet() { Data = items_tree, Type = TypePacket.SendFilesTree });
				//	}
				//	break;
				//}
				//catch { }
			}
		}
		public void Start(int port)
		{
			if (Status != StatusServer.Stop)
				return;
			Log.WriteLine("Start server", LogLevel.Warning);
			Listener = new TcpListener(port);
			Status = StatusServer.Started;
			Task.Run(() =>
			{
				Log.WriteLine("[Listener] start");
				Listener.Start();
				while (Status == StatusServer.Started)
				{
					try
					{
						Clients.Add(new Session(Listener.AcceptTcpClient()));
					}
					catch (Exception ex)
					{
						Log.WriteLine(ex, LogLevel.Error);
					}

				}
				Log.WriteLine("[Listener] stop");
			});
			Task.Run(() =>
			{
				Log.WriteLine("[Check Clients] start");
				while (Status == StatusServer.Started)
				{
					try
					{
						foreach (var i in Clients)
						{
							if (!i.Client.Connected)
							{
								i.Stop();
								Clients.Remove(i);
								break;
							}
						}
						Thread.Sleep(16);
					}
					catch (Exception ex)
					{
						Log.WriteLine(ex, LogLevel.Error);
					}

				}
				Log.WriteLine("[Check Clients] stop");
			});
		}

		public void Stop()
		{
			Log.WriteLine("Stop server", LogLevel.Warning);
			Status = StatusServer.Stop;
			Listener.Stop();
			Listener = null;
			while (true)
			{
				try
				{
					foreach (var i in Clients)
					{
						i.Stop();
					}
					break;
				}
				catch { }
			}
			
			Clients.Clear();
		}
	}
}