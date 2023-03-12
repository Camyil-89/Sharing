using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
	}
}
