using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharing.API.Models
{
	public enum Message: byte
	{
		Error = 0,
		OK = 1,
		BigSizeBlock = 2,
	}
	public class DowlaodFileInfo
	{
		public byte[] Data { get; set; } = new byte[0];

		public Message Message { get; set; } = Message.Error;
	}
}
