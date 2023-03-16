using Sharing.API;
using Sharing.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Sharing.Server
{
	public class Session
	{
		public TcpClient Client;
		private NetworkStream networkStream;
		private Dictionary<Guid, Packet> WaitPacket = new Dictionary<Guid, Packet>();

		public int TimeoutWaitPacket = 2000;
		public int TimeoutPing = 1000;
		public int Ping = -1;
		public ulong BufferSize = 2048;

		public DateTime TimeConnect;

		public EndPoint RemoteEndPoint;

		public int PortClient;
		public IPAddress IPClient;

		public Session(TcpClient client)
		{
			Client = client;
			RemoteEndPoint = Client.Client.RemoteEndPoint;
			networkStream = Client.GetStream();

			PortClient = ((IPEndPoint)RemoteEndPoint).Port;
			IPClient = ((IPEndPoint)RemoteEndPoint).Address;

			TimeConnect = DateTime.Now;
			Task.Run(HandleClient);
			Task.Run(HandleInternalRequest);
		}

		private void HandleInternalRequest()
		{
			Log.WriteLine($"[HandleInternalRequest {RemoteEndPoint}] connect", LogLevel.Warning);
			Stopwatch stopwatch_ping = Stopwatch.StartNew();
			while (Client.Connected)
			{
				if (stopwatch_ping.ElapsedMilliseconds >= TimeoutPing)
				{
					var x = SendAndWaitResponse(new Packet() { Type = TypePacket.Ping, Data = DateTime.Now });
					Ping = (int)(DateTime.Now - (DateTime)x.Data).TotalMilliseconds;
					stopwatch_ping.Restart();
				}
				Thread.Sleep(16);
			}

			Log.WriteLine($"[HandleInternalRequest {RemoteEndPoint}] disconnect", LogLevel.Warning);
		}
		private void HandleClient()
		{
			Log.WriteLine($"[HandleClient {RemoteEndPoint}] connect", LogLevel.Warning);
			while (Client.Connected)
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

					//Console.WriteLine($"[HandleClient {RemoteEndPoint}] {packet}");

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
							var items_tree = Utilities.CreateItemTree(Server.SharingFolders);
							SendPacket(new Packet() { Data = items_tree, Type = TypePacket.SendFilesTree });
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
			Log.WriteLine($"[HandleClient {RemoteEndPoint}] disconnect", LogLevel.Warning);
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
			if (Client != null && Client.Connected)
			{
				Log.WriteLine($"bytes send: {data.Length}", LogLevel.Error);
				Client.Client.Send(data);
			}
			else
				throw new Exception("Нет соединения с клиентом");
		}
		public void Stop()
		{
			Client.Close();
			Client.Dispose();
		}

		~Session()
		{
			Stop();
		}
	}
}
