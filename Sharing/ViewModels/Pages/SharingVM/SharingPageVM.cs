using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;
using Sharing.API.Models;
using Sharing.Base.Command;
using Sharing.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Sharing.ViewModels.Pages.SharingVM
{
	public class ItemClient : Base.ViewModel.BaseViewModel
	{
		#region IPaddress: Description
		/// <summary>Description</summary>
		private IPAddress _IPaddress;
		/// <summary>Description</summary>
		public IPAddress IPaddress { get => _IPaddress; set => Set(ref _IPaddress, value); }
		#endregion


		#region Port: Description
		/// <summary>Description</summary>
		private int _Port;
		/// <summary>Description</summary>
		public int Port { get => _Port; set => Set(ref _Port, value); }
		#endregion


		#region TimeConnect: Description
		/// <summary>Description</summary>
		private string _TimeConnect;
		/// <summary>Description</summary>
		public string TimeConnect { get => _TimeConnect; set => Set(ref _TimeConnect, value); }
		#endregion


		#region Ping: Description
		/// <summary>Description</summary>
		private int _Ping = -1;
		/// <summary>Description</summary>
		public int Ping { get => _Ping; set => Set(ref _Ping, value); }
		#endregion
	}
	class SharingPageVM : Base.ViewModel.BaseViewModel
	{
		private static object _lock = new object();
		public SharingPageVM()
		{
			BindingOperations.EnableCollectionSynchronization(Clients, _lock);
			#region Commands
			#endregion
		}

		#region Parametrs
		public Settings Settings => Settings.Instance;
		#region StartStopButtonText: Description
		/// <summary>Description</summary>
		private string _StartStopButtonText = "Запустить";
		/// <summary>Description</summary>
		public string StartStopButtonText { get => _StartStopButtonText; set => Set(ref _StartStopButtonText, value); }
		#endregion


		#region Clients: Description
		/// <summary>Description</summary>
		private ObservableCollection<ItemClient> _Clients = new ObservableCollection<ItemClient>();
		/// <summary>Description</summary>
		public ObservableCollection<ItemClient> Clients { get => _Clients; set => Set(ref _Clients, value); }
		#endregion


		#region SelectedPath: Description
		/// <summary>Description</summary>
		private SharingFile _SelectedPath;
		/// <summary>Description</summary>
		public SharingFile SelectedPath { get => _SelectedPath; set => Set(ref _SelectedPath, value); }
		#endregion

		#endregion


		#region Commands



		#region StartStopServerCommand: Description
		private ICommand _StartStopServerCommand;
		public ICommand StartStopServerCommand => _StartStopServerCommand ??= new LambdaCommand(OnStartStopServerCommandExecuted, CanStartStopServerCommandExecute);
		private bool CanStartStopServerCommandExecute(object e) => true;
		private void OnStartStopServerCommandExecuted(object e)
		{
			//if (Services.Net.Server.ServerProvider.GetStatusServer() == Sharing.Server.StatusServer.Stop)
			//	Services.Net.Server.ServerProvider.Start(Services.Settings.Instance.Parametrs.ServerPort);
			//else
			//	Services.Net.Server.ServerProvider.Stop();

			if (Services.Net.Server.ServerProvider.GetStatusServer() == Services.Net.Server.StatusServer.Shutdown)
				Services.Net.Server.ServerProvider.Start();
			else
				Services.Net.Server.ServerProvider.Stop();
		}
		#endregion

		#region RemoveFolderOrFileCommand: Description
		private ICommand _RemoveFolderOrFileCommand;
		public ICommand RemoveFolderOrFileCommand => _RemoveFolderOrFileCommand ??= new LambdaCommand(OnRemoveFolderOrFileCommandExecuted, CanRemoveFolderOrFileCommandExecute);
		private bool CanRemoveFolderOrFileCommandExecute(object e) => SelectedPath != null;
		private void OnRemoveFolderOrFileCommandExecuted(object e)
		{
			Settings.RemoveSharingFile(SelectedPath);
		}
		#endregion
		#region AddNewFileOrFolderCommand: Description
		private ICommand _AddNewFileCommand;
		public ICommand AddNewFileCommand => _AddNewFileCommand ??= new LambdaCommand(OnAddNewFileCommandExecuted, CanAddNewFileCommandExecute);
		private bool CanAddNewFileCommandExecute(object e) => true;
		private void OnAddNewFileCommandExecuted(object e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = true;
			openFileDialog.ShowDialog();
			if (openFileDialog.FileNames.Length > 0)
			{
				foreach (string file in openFileDialog.FileNames)
				{
					if (Settings.Parametrs.SharingFilesAndFolders.FirstOrDefault((i)=> i.Path == file) == null)
					{
						Settings.AddSharingFile(new SharingFile() { Path = file });
					}
				}
			}
		}
		#endregion

		#region AddNewFolderCommand: Description
		private ICommand _AddNewFolderCommand;
		public ICommand AddNewFolderCommand => _AddNewFolderCommand ??= new LambdaCommand(OnAddNewFolderCommandExecuted, CanAddNewFolderCommandExecute);
		private bool CanAddNewFolderCommandExecute(object e) => true;
		private void OnAddNewFolderCommandExecuted(object e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			dialog.IsFolderPicker = true;
			dialog.Multiselect= true;
			if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileNames.Count() > 0)
			{
				foreach (string file in dialog.FileNames)
				{
					if (Settings.Parametrs.SharingFilesAndFolders.FirstOrDefault((i) => i.Path == file) == null)
					{
						Settings.AddSharingFile(new SharingFile() { Path = file });
					}
				}
			}
		}
		#endregion
		#endregion

		#region Functions
		#endregion
	}
}
