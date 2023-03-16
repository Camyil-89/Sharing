using Sharing.Base.Command;
using Sharing.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Sharing.ViewModels.Pages.Dowload
{

	class DowloadPageVM : Base.ViewModel.BaseViewModel
	{
		private static object _lock = new object();
		public DowloadPageVM()
		{
			BindingOperations.EnableCollectionSynchronization(ListNodes, _lock);
			#region Commands
			#endregion
		}

		#region Parametrs


		#region StartStopButtonText: Description
		/// <summary>Description</summary>
		private string _StartStopButtonText = "Подключиться";
		/// <summary>Description</summary> 
		public string StartStopButtonText { get => _StartStopButtonText; set => Set(ref _StartStopButtonText, value); }
		#endregion


		#region ActiveStartStopButton: Description
		/// <summary>Description</summary>
		private bool _ActiveStartStopButton = true;
		/// <summary>Description</summary>
		public bool ActiveStartStopButton { get => _ActiveStartStopButton; set => Set(ref _ActiveStartStopButton, value); }
		#endregion


		#region ListNodes: Description
		/// <summary>Description</summary>
		private ObservableCollection<TreeViewItem> _ListNodes = new ObservableCollection<TreeViewItem>();
		/// <summary>Description</summary>
		public ObservableCollection<TreeViewItem> ListNodes { get => _ListNodes; set => Set(ref _ListNodes, value); }
		#endregion

		#endregion

		#region Commands


		#region StartStopClientCommand: Description
		private ICommand _StartStopClientCommand;
		public ICommand StartStopClientCommand => _StartStopClientCommand ??= new LambdaCommand(OnStartStopClientCommandExecuted, CanStartStopClientCommandExecute);
		private bool CanStartStopClientCommandExecute(object e) => true;
		private void OnStartStopClientCommandExecuted(object e)
		{
			if (!Services.Net.Client.ClientProvider.IsActive)
				Services.Net.Client.ClientProvider.Start(new System.Net.IPAddress(new byte[] {127, 0, 0, 1}), Settings.Instance.Parametrs.ConnectPortServer);
			else
				Services.Net.Client.ClientProvider.Stop();
		}
		#endregion
		#endregion

		#region Functions
		#endregion
	}
}
