using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharing.API.Models
{
	public class RequestFileInfo
	{
		public string RootPath { get; set; }
		public string UID_ROOT {get; set; }
		public string Name { get; set; }
		public string Path { get; set; }
		public bool IsFinish { get; set; }
		public long DowloadSize { get; set; } = 0;
		public long TotalSize { get; set; }
		public int BlockSize { get; set; } = 2048;
	}
}
