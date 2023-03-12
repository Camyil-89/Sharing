using Sharing.Base.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sharing.ViewModels.Pages.Dowload
{

	class DowloadPageVM : Base.ViewModel.BaseViewModel
	{
		public DowloadPageVM()
		{
			#region Commands
			#endregion
		}

		#region Parametrs

		#region Client: Description
		/// <summary>Description</summary>
		private Client.Client _Client = new Client.Client();
		/// <summary>Description</summary>
		public Client.Client Client { get => _Client; set => Set(ref _Client, value); }
		#endregion
		#endregion

		#region Commands

		#region StartServerCommand: Description
		private ICommand _StartServerCommand;
		public ICommand StartServerCommand => _StartServerCommand ??= new LambdaCommand(OnStartServerCommandExecuted, CanStartServerCommandExecute);
		private bool CanStartServerCommandExecute(object e) => true;
		private void OnStartServerCommandExecuted(object e)
		{
			Client.Start(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 3000);
		}
		#endregion
		#endregion

		#region Functions
		#endregion
	}
}
