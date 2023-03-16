using Microsoft.Extensions.DependencyInjection;
using Sharing.Base.Command;
using Sharing.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sharing.ViewModels.Windows.Main
{

	class MainWindowVM : Base.ViewModel.BaseViewModel
	{
		public MainWindowVM()
		{
			#region Commands
			#endregion
		}

		#region Parametrs
		public Settings Settings => Settings.Instance;

		#region FocusSharingMenu: Description
		/// <summary>Description</summary>
		private string _FocusSharingMenu = "focus";
		/// <summary>Description</summary>
		public string FocusSharingMenu { get => _FocusSharingMenu; set => Set(ref _FocusSharingMenu, value); }
		#endregion


		#region FocusDowloadMenu: Description
		/// <summary>Description</summary>
		private string _FocusDowloadMenu;
		/// <summary>Description</summary>
		public string FocusDowloadMenu { get => _FocusDowloadMenu; set => Set(ref _FocusDowloadMenu, value); }
		#endregion


		#region FocusSettingsMenu: Description
		/// <summary>Description</summary>
		private string _FocusSettingsMenu;
		/// <summary>Description</summary>
		public string FocusSettingsMenu { get => _FocusSettingsMenu; set => Set(ref _FocusSettingsMenu, value); }
		#endregion


		#region SelectedPage: Description
		/// <summary>Description</summary>
		private Page _SelectedPage = App.Host.Services.GetRequiredService<Views.Pages.Sharing.SharingPage>();
		/// <summary>Description</summary>
		public Page SelectedPage { get => _SelectedPage; set => Set(ref _SelectedPage, value); }
		#endregion
		#endregion


		#region IPaddressServer: Description
		/// <summary>Description</summary>
		private string _IPaddressServer = "";
		/// <summary>Description</summary>
		public string IPaddressServer { get => _IPaddressServer; set => Set(ref _IPaddressServer, value); }
		#endregion


		#region VisibilityServerStatus: Description
		/// <summary>Description</summary>
		private Visibility _VisibilityServerStatus = Visibility.Collapsed;
		/// <summary>Description</summary>
		public Visibility VisibilityServerStatus { get => _VisibilityServerStatus; set => Set(ref _VisibilityServerStatus, value); }
		#endregion

		#region Commands

		#region OpenMenuCommand: Description
		private ICommand _OpenMenuCommand;
		public ICommand OpenMenuCommand => _OpenMenuCommand ??= new LambdaCommand(OnOpenMenuCommandExecuted, CanOpenMenuCommandExecute);
		private bool CanOpenMenuCommandExecute(object e) => true;
		private void OnOpenMenuCommandExecuted(object e)
		{
			OpenMenu((e as Page));
		}
		#endregion
		#endregion

		#region Functions
		public void OpenMenu(Page page)
		{
			SelectedPage = page;

			FocusSharingMenu = SelectedPage.Equals(App.Host.Services.GetRequiredService<Views.Pages.Sharing.SharingPage>()) == true ? "focus": "";
			FocusDowloadMenu = SelectedPage.Equals(App.Host.Services.GetRequiredService<Views.Pages.Dowload.DowloadPage>()) == true ? "focus": "";
		}
		#endregion
	}
}
