using Sharing.API;
using Sharing.Services;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Sharing.Client
{
	public enum StatusClient : byte
	{
		Work = 1,
		Stop = 0,
	}
	public class Client
	{
		private TcpClient _Client;

		public int Ping { get; private set; } = -1;
		public StatusClient Status { get; private set; } = StatusClient.Stop;

		private DateTime PingDateTime;
		private Dictionary<Guid, Packet> WaitPacket = new Dictionary<Guid, Packet>();
		public bool Start(IPAddress IPAddress, int port)
		{
			if (Status != StatusClient.Stop)
				return false;
			Log.WriteLine("Start client", LogLevel.Warning);
			Status = StatusClient.Work;
			_Client = new TcpClient();
			try
			{
				_Client.Connect(IPAddress, port);
			}
			catch { return false; }
			Task.Run(() =>
			{
				StartListen();
			});
			return true;
		}
		private void StartListen()
		{
			NetworkStream networkStream = _Client.GetStream();

			Task.Run(() =>
			{
				Stopwatch stopwatch_ping = Stopwatch.StartNew();
				while (_Client.Connected && Status == StatusClient.Work)
				{
					try
					{
						if (stopwatch_ping.ElapsedMilliseconds >= 1000)
						{
							var x = SendAndWaitResponse(new Packet() { Type = TypePacket.Ping, Data = DateTime.Now });
							stopwatch_ping.Restart();
						}
					}
					catch { }
				}
			});
			while (_Client.Connected && Status == StatusClient.Work)
			{
				try
				{
					byte[] myReadBuffer = new byte[2048];
					List<byte> allData = new List<byte>();
					int numBytesRead = networkStream.Read(myReadBuffer, 0, myReadBuffer.Length);
					if (numBytesRead == myReadBuffer.Length) allData.AddRange(myReadBuffer);
					else if (numBytesRead > 0) allData.AddRange(myReadBuffer.Take(numBytesRead));

					Packet packet = Packet.FromByteArray(allData.ToArray());

					Console.WriteLine($"[CLIENT] {packet}");

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

			return WaitPacket[packet.UID];
		}
		public void SendPacket(Packet packet)
		{
			SendBytes(Packet.ToByteArray(packet));
		}
		public void SendBytes(byte[] bytes)
		{
			if (_Client.Connected && Status == StatusClient.Work)
			{
				_Client.Client.Send(bytes);
			}
		}
		public void Stop()
		{
			Log.WriteLine("Stop client", LogLevel.Warning);
			Status = StatusClient.Stop;
		}

	}
}