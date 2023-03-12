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
		private TcpClient Client;

		public StatusServer Status { get; private set; } = StatusServer.Stop;

		public int Ping { get; private set; } = -1;

		private List<string> SharingFolders = new List<string>();
		private DateTime PingDateTime;

		private Dictionary<Guid, Packet> WaitPacket = new Dictionary<Guid, Packet>();
		public void SetSharingFolder(List<string> folders)
		{
			SharingFolders = folders;

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
				Listener.Start();
				while (Status == StatusServer.Started)
				{
					try
					{
						Client = Listener.AcceptTcpClient();
						AcceptClient();
					}
					catch (Exception ex)
					{
						Log.WriteLine(ex, LogLevel.Error);
					}

				}
			});
		}

		private void AcceptClient()
		{
			Log.WriteLine($"AcceptClient: {Client.Client.RemoteEndPoint}", LogLevel.Info);
			NetworkStream networkStream = Client.GetStream();
			Task.Run(() =>
			{
				Stopwatch stopwatch_ping = Stopwatch.StartNew();
				while (Client.Connected && Status == StatusServer.Started)
				{
					Thread.Sleep(1);
					try
					{
						if (stopwatch_ping.ElapsedMilliseconds >= 1000)
						{
							var x = SendAndWaitResponse(new Packet() { Type = TypePacket.Ping, Data = DateTime.Now });
							stopwatch_ping.Restart();
						}
					}
					catch (Exception ex) { Console.WriteLine(ex); }
				}
			});
			while (Client.Connected && Status == StatusServer.Started)
			{
				try
				{
					byte[] myReadBuffer = new byte[2048];
					List<byte> allData = new List<byte>();
					int numBytesRead = networkStream.Read(myReadBuffer, 0, myReadBuffer.Length);
					if (numBytesRead == myReadBuffer.Length) allData.AddRange(myReadBuffer);
					else if (numBytesRead > 0) allData.AddRange(myReadBuffer.Take(numBytesRead));

					Packet packet = Packet.FromByteArray(allData.ToArray());

					Console.WriteLine($"[SERVER] {packet}");

					if (WaitPacket.ContainsKey(packet.UID))
					{
						WaitPacket[packet.UID] = packet;
						continue;
					}


					switch (packet.Type)
					{
						case TypePacket.SendFile:
							break;
						case TypePacket.SendFilesTree:
							break;
						case TypePacket.Ping:
							SendPacket(packet);
							break;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
		}
		public Packet SendAndWaitResponse(Packet packet)
		{
			WaitPacket.Add(packet.UID, null);
			SendPacket(packet);
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (WaitPacket[packet.UID] == null)
			{
				if (stopwatch.ElapsedMilliseconds > 2000)
					throw new Exception("Timeout");
			}
			var x = WaitPacket[packet.UID];
			WaitPacket.Remove(packet.UID);
			return x;
		}
		public void SendPacket(Packet packet)
		{
			SendBytes(Packet.ToByteArray(packet));
		}
		public void SendBytes(byte[] data)
		{
			if (Client != null)
			{
				Client.Client.Send(data);
			}
		}

		public void Stop()
		{
			Log.WriteLine("Stop server", LogLevel.Warning);
			Listener.Stop();
			Listener = null;
			Client.Close();
			Client.Dispose();
			Client = null;
			Status = StatusServer.Stop;
		}
	}
}