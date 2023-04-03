using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharing.API.Models
{
	public class DowlaodFileInfo
	{
		public byte[] Data { get; set; }
		public int BufferSize { get; set; }
		public int ReadBytes { get; set; }

	}
}
