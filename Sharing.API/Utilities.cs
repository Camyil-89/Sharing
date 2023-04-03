using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Sharing.API.Models;

namespace Sharing.API
{
	[Serializable]
	public class ItemTree
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public string UID_ROOT { get; set; }

		public long Size { get; set; } = 0;
		public bool IsFolder { get; set; } = false;

		public List<ItemTree> ItemsTrees { get; set; } = new List<ItemTree>();
	}
	public static class Utilities
	{
		public static List<UnicastIPAddressInformation> GetLocalIPAddresses()
		{
			NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
			List<UnicastIPAddressInformation> addresses = new List<UnicastIPAddressInformation>();
			foreach (NetworkInterface ni in interfaces)
			{
				if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
					ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
				{
					IPInterfaceProperties ipProps = ni.GetIPProperties();
					foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
					{
						if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						{
							addresses.Add(addr);
						}
					}
				}
			}
			return addresses;
		}
		public static string GetStringSha256Hash(string text)
		{
			if (String.IsNullOrEmpty(text))
				return String.Empty;

			using (var sha = new System.Security.Cryptography.SHA256Managed())
			{
				byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
				byte[] hash = sha.ComputeHash(textData);
				return BitConverter.ToString(hash).Replace("-", String.Empty);
			}
		}
		public static string GetSharingFilePath(List<SharingFile> sharings, RequestFileInfo requestFile)
		{
			foreach (var i in sharings)
			{
				if (string.IsNullOrEmpty(requestFile.Path))
				{
					if (GetStringSha256Hash(i.Path) == requestFile.UID_ROOT)
					{
						return i.Path;
						break;
					}
				}
				else
				{
					if (GetStringSha256Hash(i.Path.Replace(requestFile.Path, "")) == requestFile.UID_ROOT)
					{
						return i.Path;
					}
				}
			}
			return "";
		}
		public static string RoundByte(long Bytes)
		{
			if (Bytes < Math.Pow(1024, 1)) return $"{Bytes} Б";
			else if (Bytes < Math.Pow(1024, 2)) return $"{Math.Round((float)Bytes / 1024, 2)} Кб";
			else if (Bytes < Math.Pow(1024, 3)) return $"{Math.Round((float)Bytes / Math.Pow(1024, 2), 2)} Мб";
			else if (Bytes < Math.Pow(1024, 4)) return $"{Math.Round((float)Bytes / Math.Pow(1024, 3), 2)} Гб";
			return "";
		}
		private static ItemTree CreateItem(string path, string root_path)
		{
			ItemTree item = new ItemTree();
			item.IsFolder = Directory.Exists(path);
			item.Name = path.Split("\\").Last();
			item.Path = path.Replace(root_path, "");
			item.UID_ROOT = GetStringSha256Hash(root_path);
			if (!item.IsFolder)
			{
				FileInfo fileInfo = new FileInfo(path);
				item.Size = fileInfo.Length;
			}
			else
			{
				foreach (var i in Directory.GetDirectories(path))
				{
					item.ItemsTrees.Add(CreateItem(i, root_path));
				}
				foreach (var i in Directory.GetFiles(path))
				{
					item.ItemsTrees.Add(CreateItem(i, root_path));
				}
			}
			return item;
		}
		public static List<ItemTree> CreateItemTree(List<SharingFile> items)
		{
			List<ItemTree> list_itemTree = new List<ItemTree>();
			foreach (var file in items)
			{
				list_itemTree.Add(CreateItem(file.Path, file.Path));
			}
			return list_itemTree;
		}
	}
}
