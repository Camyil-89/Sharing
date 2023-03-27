using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Sharing.API
{
	public class PacketSender
	{
		public PacketSender(TcpClient stream)
		{
			Client = stream;
		}
		private List<byte[]> Packets = new List<byte[]>();
		public TcpClient Client { get; }

		public int GetCountPacketNotSend()
		{
			return Packets.Count;
		}

		public void Send(byte[] packet)
		{
			Packets.Add(packet);
		}
		public void Loop()
		{
			if (Packets.Count > 0)
			{
				Client.Client.Send(Packets[0]);
				Packets.RemoveAt(0);
			}
		}
	}
}
