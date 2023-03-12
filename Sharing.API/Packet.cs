using System.Runtime.Serialization.Formatters.Binary;

namespace Sharing.API
{
	public enum TypePacket: byte
	{
		SendFile = 0,
		SendFilesTree = 1,
		Ping = 2,
	}
	[Serializable]
	public class Packet
	{
		public Guid UID = Guid.NewGuid();
		public TypePacket Type;
		public int Version { get; set; } = 1;
		public object Data { get; set; } = null;

		public override string ToString()
		{
			return $"Version: {Version}; Data: {Data}; Type: {Type}; UID: {UID}";
		}
		public static byte[] ToByteArray(object obj)
		{
			if (obj == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter();
			byte[] array;
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				array = ms.ToArray();
			}
			return array;
		}

		public static Packet FromByteArray(byte[] data)
		{
			if (data == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream(data))
			{
				object obj = bf.Deserialize(ms);
				return (Packet)obj;
			}
		}
	}
}