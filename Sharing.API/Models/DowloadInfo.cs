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

		public void StartDowload(Requests requests, int buffer_size)
		{
			foreach (var file in Files)
			{
				if (file.IsFinish)
					continue;

				int DowloadSize = 0;
				//Stopwatch stopwatch = Stopwatch.StartNew();
				//Stopwatch stopwatch_1 = Stopwatch.StartNew();
				try
				{
					foreach (var i in requests.DowloadFile(file, buffer_size))
					{
						DowloadSize += i.ReadBytes;
						//Console.WriteLine($"{file.UID_ROOT}\\{file.Path}>{i.ReadBytes}");
						if (DowloadSize > file.BlockSize * file.StartBlock)
						{
							file.StartBlock = DowloadSize / file.BlockSize;
							//Console.WriteLine($"{DowloadSize}|{file.StartBlock}|{file.BlockSize * file.StartBlock}");
						}
						//SpeedDowload = (long)(DowloadSize / stopwatch.Elapsed.TotalSeconds);
						//if (stopwatch_1.ElapsedMilliseconds >= 10)
						//{
						//	stopwatch_1.Restart();
						//	Console.WriteLine(Utilities.RoundByte(SpeedDowload));
						//}
						//Thread.Sleep(1);
					}
					file.IsFinish = true;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					Console.WriteLine($"{DowloadSize}|{file.BlockSize}|{DowloadSize / file.BlockSize}");
					file.StartBlock = DowloadSize / file.BlockSize;
				}

			}
		}
	}
}
