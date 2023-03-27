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

		private Dictionary<Guid, Packet> WaitPacket = new Dictionary<Guid, Packet>();

		public int TimeoutWaitPacket = 2000;
		public int TimeoutPing = 1000;
		public ulong BufferSize = 2048;

		public IPAddress IPAddress;
		public int Port;

		private EndPoint RemoteEndPoint;

		public delegate void CallBackPacketFunc(Packet packet);
		public CallBackPacketFunc CallBackPacket = null;

		private PacketSender PacketSender;

		public bool Connected()
		{
			if (_Client == null)
				return false;
			return _Client.Connected;
		}

		public bool Start(IPAddress adress, int port)
		{
			if (Status != StatusClient.Stop)
				return false;
			_Client = new TcpClient();
			IPAddress = adress;
			Port = port;
			try
			{
				_Client.Connect(IPAddress, port);
				RemoteEndPoint = _Client.Client.RemoteEndPoint;
				PacketSender = new PacketSender(_Client);
			}
			catch { return false; }
			Log.WriteLine("Start client", LogLevel.Warning);
			Status = StatusClient.Work;
			SendPacket(new Packet() { Type = TypePacket.SendFilesTree });
			Task.Run(HandlerClient);
			Task.Run(HandlerInternalRequest);
			Task.Run(HandleSender);
			return true;
		}
		private void HandleSender()
		{
			while (_Client.Connected)
			{
				try
				{
					PacketSender.Loop();
				}
				catch (Exception ex) { Log.WriteLine(ex, LogLevel.Error); }
			}
		}
		private void HandlerInternalRequest()
		{
			Log.WriteLine($"[Client] [HandlerInternalRequest {RemoteEndPoint}] connect", LogLevel.Warning);
			Stopwatch stopwatch_ping = Stopwatch.StartNew();
			while (_Client.Connected && Status == StatusClient.Work)
			{
				try
				{
					if (stopwatch_ping.ElapsedMilliseconds >= TimeoutPing)
					{
						var x = SendAndWaitResponse(new Packet() { Type = TypePacket.Ping, Data = DateTime.Now });
						Ping = (int)(DateTime.Now - (DateTime)x.Data).TotalMilliseconds;
						stopwatch_ping.Restart();
					}
				}
				catch (Exception ex) { Log.WriteLine(ex, LogLevel.Error); }
				Thread.Sleep(1);
			}
			Status = StatusClient.Stop;
			Log.WriteLine($"[Client] [HandlerInternalRequest {RemoteEndPoint}] disconnect", LogLevel.Warning);
		}
		private void HandlerClient()
		{
			Log.WriteLine($"[Client] [HandlerClient {RemoteEndPoint}] connect", LogLevel.Warning);
			NetworkStream networkStream = _Client.GetStream();
			while (_Client.Connected && Status == StatusClient.Work)
			{
				try
				{
					List<byte> allData = new List<byte>();
					do
					{
						byte[] myReadBuffer = new byte[BufferSize];
						int numBytesRead = networkStream.Read(myReadBuffer, 0, myReadBuffer.Length);
						if (numBytesRead == myReadBuffer.Length) allData.AddRange(myReadBuffer);
						else if (numBytesRead > 0) allData.AddRange(myReadBuffer.Take(numBytesRead));
					} while (networkStream.DataAvailable);
					

					Packet packet = Packet.FromByteArray(allData.ToArray());

					//Console.WriteLine($"[CLIENT] {packet}");

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
							SendCallBack(packet);
							break;
						case TypePacket.Ping:
							SendPacket(packet);
							break;
					}
				}
				catch (Exception ex)
				{
					
				}
			}
			Status = StatusClient.Stop;
			Log.WriteLine($"[Client] [HandlerClient {RemoteEndPoint}] disconnect", LogLevel.Warning);
		}
		private void SendCallBack(Packet packet)
		{
			if (CallBackPacket != null)
				CallBackPacket.Invoke(packet);
		}
		public Packet SendAndWaitResponse(Packet packet)
		{
			WaitPacket.Add(packet.UID, null);
			SendPacket(packet);
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (WaitPacket[packet.UID] == null)
			{
				if (stopwatch.ElapsedMilliseconds > TimeoutWaitPacket)
					throw new Exception($"Timeout: {stopwatch.ElapsedMilliseconds}\\{TimeoutWaitPacket}");
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
				PacketSender.Send(bytes);
			}
		}
		public void Stop()
		{
			Log.WriteLine("Stop client", LogLevel.Warning);
			Status = StatusClient.Stop;
			if (_Client != null)
			{
				_Client.Close();
				_Client.Dispose();
			}
		}
		~Client()
		{
			Stop();
		}
	}
}