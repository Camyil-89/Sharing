using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace NetFind.Packet
{
	public enum FindType : int
	{
		Find = 1,
		ConnectAddress = 2,
	}
	[Serializable]
	public class UdpFind
	{
		public float Version { get; } = 1.0f;
		public string ID { get; set; }
		public FindType Type { get; set; }
		public long IPAddressServer { get; set; }
		public int PortServer { get; set; }

		public override string ToString()
		{
			return $"Type: {Type};IPAddressServer: {IPAddressServer};Version: {Version};ID: {ID}";
		}
		public static byte[] ToByteArray(object obj)
		{
			if (obj == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}
		public static UdpFind FromByteArray(byte[] data)
		{
			if (data == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream(data))
			{
				object obj = bf.Deserialize(ms);
				return (UdpFind)obj;
			}
		}
	}
}