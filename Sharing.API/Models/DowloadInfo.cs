using Sharing.API.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharing.API.Models
{
	public class DowloadInfo
	{
		public List<RequestFileInfo> Files { get; set; } = new List<RequestFileInfo>();

		public long SpeedDowload = 0;

		public void StartDowload(Requests requests, int buffer_size, string path)
		{
			foreach (var file in Files)
			{
				if (file.IsFinish)
					continue;

				int DowloadSize = 0;
				FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
				fileStream.Position = file.BlockSize * file.StartBlock;
				//Stopwatch stopwatch = Stopwatch.StartNew();
				//Stopwatch stopwatch_1 = Stopwatch.StartNew();
				try
				{
					bool IsError = false;
					while (DowloadSize != file.TotalSize)
					{
						Stopwatch stopwatch = Stopwatch.StartNew();
						var fileinfo = requests.DowloadFile(file, buffer_size);
						if (fileinfo.Message != Message.OK)
						{
							IsError = true;
							break;
						}
						fileStream.Write(fileinfo.Data, 0, fileinfo.Data.Length);
						DowloadSize += fileinfo.Data.Length;

						if (DowloadSize >= file.BlockSize * file.StartBlock)
						{
							file.StartBlock = DowloadSize / file.BlockSize;
						}
						Console.WriteLine($"{Utilities.RoundByte((long)(fileinfo.Data.Length / stopwatch.Elapsed.TotalSeconds))}");
					}
					if (!IsError)
						file.IsFinish = true;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					file.StartBlock = DowloadSize / file.BlockSize;
				}

			}
		}
	}
}
