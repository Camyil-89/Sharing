using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharing.API
{
	[Serializable]
	public class ItemTree
	{
		public string Name { get; set; }
		public string UID { get; set; }

		public long Size { get; set; } = 0;
		public bool IsFolder { get; set; } = false;

		public List<ItemTree> ItemsTrees { get; set; } = new List<ItemTree>();
	}
	public static class Utilities
	{
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
		private static ItemTree CreateItem(string path)
		{
			ItemTree item = new ItemTree();
			item.IsFolder = Directory.Exists(path);
			item.Name = path.Split("\\").Last();
			item.UID = GetStringSha256Hash(path);
			if (!item.IsFolder)
			{
				FileInfo fileInfo = new FileInfo(path);
				item.Size = fileInfo.Length;
			}
			else
			{
				foreach (var i in Directory.GetFiles(path))
				{
					item.ItemsTrees.Add(CreateItem(i));
				}
				foreach (var i in Directory.GetDirectories(path))
				{
					item.ItemsTrees.Add(CreateItem(i));
				}
			}
			return item;
		}
		public static List<ItemTree> CreateItemTree(List<string> items)
		{
			List<ItemTree> list_itemTree = new List<ItemTree>();
			foreach (var path in items)
			{
				list_itemTree.Add(CreateItem(path));
			}
			return list_itemTree;
		}
	}
}
