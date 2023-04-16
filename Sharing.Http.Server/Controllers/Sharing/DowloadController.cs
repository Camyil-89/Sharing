using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sharing.API;
using Sharing.API.Models;
using Sharing.Http.Server.Services;
using Sharing.Services;
using System.IO;
using System.Net;
using System.Text;

namespace Sharing.Http.Server.Controllers.Sharing
{
	[ApiController]
	public class DowloadController : ControllerBase
	{
		[Route("api/[controller]/files")]
		[HttpGet]
		public async Task<DowlaodFileInfo> Dowload()
		{
			try
			{
				var file = await HttpContext.Request.ReadFromJsonAsync<API.Models.RequestFileInfo>();
				if (file.BlockSize > Settings.Server.MaxSizeBlock)
					return new DowlaodFileInfo() { Message = Message.BigSizeBlock };
				var path = Utilities.GetSharingFilePath(Settings.SharingFiles, file);
				if (!System.IO.File.Exists(path))
					return new DowlaodFileInfo();

				if (path.Contains("..\\"))
					return new DowlaodFileInfo() { Message = Message.ErrorPath};

				var file_str = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096);
				file_str.Position = file.BlockSize * file.StartBlock;

				byte[] buffer = new byte[file.BlockSize * Settings.Server.SizeBuffer];

				if (buffer.Length > file.TotalSize - file.BlockSize * file.StartBlock)
					buffer = new byte[file.TotalSize - file.BlockSize * file.StartBlock];
				file_str.Read(buffer, 0, buffer.Length);

				//Console.WriteLine($">{path}|{buffer.Length}|{file.TotalSize}|{file.BlockSize * file.StartBlock}|{file.TotalSize - file.BlockSize * file.StartBlock}|");
				return new DowlaodFileInfo() { Data = buffer, Message = Message.OK };
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return new DowlaodFileInfo();
			}

		}
	}
}
