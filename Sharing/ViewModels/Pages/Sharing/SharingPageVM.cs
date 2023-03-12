using Sharing.Base.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sharing.ViewModels.Pages.Sharing
{

	class SharingPageVM : Base.ViewModel.BaseViewModel
	{
		public SharingPageVM()
		{
			#region Commands
			#endregion
		}

		#region Parametrs

		#region Server: Description
		/// <summary>Description</summary>
		private Server.Server _Server = new Server.Server();
		/// <summary>Description</summary>
		public Server.Server Server { get => _Server; set => Set(ref _Server, value); }
		#endregion
		#endregion

		#region Commands

		#region StartClientCommand: Description
		private ICommand _StartClientCommand;
		public ICommand StartClientCommand => _StartClientCommand ??= new LambdaCommand(OnStartClientCommandExecuted, CanStartClientCommandExecute);
		private bool CanStartClientCommandExecute(object e) => true;
		private void OnStartClientCommandExecuted(object e)
		{
			Server.Start(3000);
		}
		#endregion
		#endregion

		#region Functions
		#endregion
	}
}
