using Microsoft.Extensions.DependencyInjection;
using Sharing.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sharing.Services
{
    public class Settings: Base.ViewModel.BaseViewModel
    {
		public static Settings Instance => App.Host.Services.GetRequiredService<Settings>();

		private System.Version _Version = Assembly.GetEntryAssembly().GetName().Version;
		/// <summary>версия приложения</summary>
		public System.Version Version { get => _Version; set => Set(ref _Version, value); }




		#region Parametrs: Description
		/// <summary>Description</summary>
		private Models.Parametrs _Parametrs = new Models.Parametrs();
		/// <summary>Description</summary>
		public Models.Parametrs Parametrs { get => _Parametrs; set => Set(ref _Parametrs, value); }
		#endregion

		private List<string> GetAllFiles(string directoryPath)
		{
			List<string> fileList = new List<string>();

			foreach (string file in Directory.GetFiles(directoryPath))
			{
				fileList.Add(file);
			}

			foreach (string subdirectory in Directory.GetDirectories(directoryPath))
			{
				fileList.AddRange(GetAllFiles(subdirectory));
			}
			fileList.Add(directoryPath);
			return fileList;
		}
		public void AddSharingFile(SharingFile file)
		{
			foreach (var i in Parametrs.SharingFilesAndFolders)
			{
				if (file.Path.Contains(i.Path))
				{
					return;
				}
			}
			List<SharingFile> removes = new List<SharingFile>();
			foreach (var i in Parametrs.SharingFilesAndFolders)
			{
				if (i.Path.Contains(file.Path))
					removes.Add(i);
			}
			foreach (var i in removes)
				Parametrs.SharingFilesAndFolders.Remove(i);


			Parametrs.SharingFilesAndFolders.Add(file);
			Net.Server.ServerProvider.SetSharingFolder();
		}
		public void RemoveSharingFile(SharingFile file)
		{
			Parametrs.SharingFilesAndFolders.Remove(file);
			Net.Server.ServerProvider.SetSharingFolder();
		}
	}
}
