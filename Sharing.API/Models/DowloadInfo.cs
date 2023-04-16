using Sharing.API.Net;
using Sharing.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharing.API.Models
{
	public class StatsDowload
	{
		public long TotalSize { get; set; }
		public long DowloadSize { get; set; }
		public string Path { get; set; } = "";
		public long SpeedDowload { get; set; }
		public string CountFilesDowloadAndNeedDowload { get; set; } = "";
	}
	public class DowloadInfo
	{
		public List<RequestFileInfo> Files { get; set; } = new List<RequestFileInfo>();
		public List<StatsDowload> StatsDowloads { get; set; } = new List<StatsDowload>();
		public string PathSaveProgress { get; set; }

		public long SpeedDowload = 0;
		public bool IsDowload = false;

		public bool Abort = false;

		public bool Dowload(RequestFileInfo file, Requests requests)
		{
			if (file.IsFinish)
				return true;
			StatsDowload stats = new StatsDowload();
			StatsDowloads.Add(stats);
			string path = $"{file.RootPath}{file.Path}";
			if (string.IsNullOrEmpty(file.Path))
			{
				path = $"{file.RootPath}\\{file.Name}";
			}
			stats.Path = path;
			stats.TotalSize = file.TotalSize;
			stats.CountFilesDowloadAndNeedDowload = $"{StatsDowloads.Count} \\ {Files.Count}";
			FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
			fileStream.Position = file.BlockSize * file.StartBlock;
			long DowloadSize = fileStream.Position;
			try
			{
				bool IsError = false;
				while (DowloadSize != file.TotalSize && Abort == false)
				{
					Stopwatch stopwatch = Stopwatch.StartNew();
					var fileinfo = requests.DowloadFile(file);
					if (fileinfo.Message != Message.OK)
					{
						IsError = true;
						break;
					}
					fileStream.Write(fileinfo.Data, 0, fileinfo.Data.Length);
					DowloadSize += fileinfo.Data.Length;

					if (DowloadSize >= file.BlockSize * file.StartBlock)
					{
						file.StartBlock = (int)(DowloadSize / file.BlockSize);
						SaveInfo();
					}
					SpeedDowload = (long)(fileinfo.Data.Length / stopwatch.Elapsed.TotalSeconds);
					stats.DowloadSize = DowloadSize;
					stats.SpeedDowload = SpeedDowload;
					fileinfo.Data = null;
				}
				if (!IsError && Abort == false)
					file.IsFinish = true;
				SaveInfo();
				fileStream.Close();
				return file.IsFinish;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				file.StartBlock = (int)(DowloadSize / file.BlockSize);
			}
			fileStream.Close();
			SaveInfo();
			return false;
		}
		public void StartDowload(Requests requests)
		{
			int count = 0;
			IsDowload = false;
			foreach (var file in Files)
			{
				if (Abort)
					break;
				if (Dowload(file, requests))
					count++;
			}
			IsDowload = count == Files.Count;
		}
		public void SaveInfo()
		{
			if (File.Exists(PathSaveProgress))
				XMLProvider.Save<List<RequestFileInfo>>(PathSaveProgress, Files);
		}
		public void LoadInfo()
		{
			Files = XMLProvider.Load<List<RequestFileInfo>>(PathSaveProgress);
		}
	}
}
