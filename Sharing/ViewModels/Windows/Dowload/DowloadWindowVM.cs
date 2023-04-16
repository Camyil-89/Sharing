using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using Sharing.API;
using Sharing.API.Models;
using Sharing.Base.Command;
using Sharing.Http.Client;
using Sharing.Models;
using Sharing.Services;
using Sharing.Services.Net.Client;
using Sharing.Views.Windows.Dowload;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Sharing.ViewModels.Windows.Dowload
{
	public class Info : Base.ViewModel.BaseViewModel
	{

		#region Name: Description
		/// <summary>Description</summary>
		private string _Name;
		/// <summary>Description</summary>
		public string Name { get => _Name; set => Set(ref _Name, value); }
		#endregion


		#region TotalSize: Description
		/// <summary>Description</summary>
		private long _TotalSize = 0;
		/// <summary>Description</summary>
		public long TotalSize { get => _TotalSize; set => Set(ref _TotalSize, value); }
		#endregion


		#region DowloadSize: Description
		/// <summary>Description</summary>
		private long _DowloadSize = 0;
		/// <summary>Description</summary>
		public long DowloadSize { get => _DowloadSize; set => Set(ref _DowloadSize, value); }
		#endregion


		#region TextSpeedDowload: Description
		/// <summary>Description</summary>
		private string _TextSpeedDowload;
		/// <summary>Description</summary>
		public string TextSpeedDowload { get => _TextSpeedDowload; set => Set(ref _TextSpeedDowload, value); }
		#endregion
	}
	public enum ModeWindow : byte
	{
		SelectFileDowload = 0,
		DowloadWindow = 1,
	}
	class DowloadWindowVM : Base.ViewModel.BaseViewModel
	{
		private static object _lock = new object();
		public DowloadWindowVM()
		{
			BindingOperations.EnableCollectionSynchronization(Nodes, _lock);
			#region Commands
			#endregion
		}

		#region Parametrs

		public DowloadWindow Window;

		#region DowloadNode: Description
		/// <summary>Description</summary>
		private ItemTree _DowloadNode;
		/// <summary>Description</summary>
		public ItemTree DowloadNode { get => _DowloadNode; set => Set(ref _DowloadNode, value); }
		#endregion


		#region Nodes: Description
		/// <summary>Description</summary>
		private ObservableCollection<Models.TreeViewItemFile> _Nodes = new ObservableCollection<Models.TreeViewItemFile>();
		/// <summary>Description</summary>
		public ObservableCollection<Models.TreeViewItemFile> Nodes { get => _Nodes; set => Set(ref _Nodes, value); }
		#endregion


		#region VisibilitySelectFile: Description
		/// <summary>Description</summary>
		private Visibility _VisibilitySelectFile = Visibility.Hidden;
		/// <summary>Description</summary>
		public Visibility VisibilitySelectFile { get => _VisibilitySelectFile; set => Set(ref _VisibilitySelectFile, value); }
		#endregion

		#region VisibilityDowloadWindow: Description
		/// <summary>Description</summary>
		private Visibility _VisibilityDowloadWindow = Visibility.Hidden;
		/// <summary>Description</summary>
		public Visibility VisibilityDowloadWindow { get => _VisibilityDowloadWindow; set => Set(ref _VisibilityDowloadWindow, value); }

		#endregion
		#region PathToDowload: Description
		/// <summary>Description</summary>
		private string _PathToDowload;
		/// <summary>Description</summary>
		public string PathToDowload { get => _PathToDowload; set => Set(ref _PathToDowload, value); }
		#endregion
		#endregion

		private List<RequestFileInfo> DowloadItems = new List<RequestFileInfo>();
		private DowloadInfo DowloadInfo = new DowloadInfo();
		private int ServerBlockSize = 0;
		private Stopwatch StopwatchTimeDowload = new Stopwatch();

		#region MaximumSize: Description
		/// <summary>Description</summary>
		private long _MaximumSize;
		/// <summary>Description</summary>
		public long MaximumSize { get => _MaximumSize; set => Set(ref _MaximumSize, value); }
		#endregion


		#region NowValue: Description
		/// <summary>Description</summary>
		private long _NowValue;
		/// <summary>Description</summary>
		public long NowValue { get => _NowValue; set => Set(ref _NowValue, value); }
		#endregion

		#region TextSpeed: Description
		/// <summary>Description</summary>
		private string _TextSpeed;
		/// <summary>Description</summary>
		public string TextSpeed { get => _TextSpeed; set => Set(ref _TextSpeed, value); }
		#endregion

		#region NowPath: Description
		/// <summary>Description</summary>
		private string _NowPath;
		/// <summary>Description</summary>
		public string NowPath { get => _NowPath; set => Set(ref _NowPath, value); }
		#endregion


		#region DowloadFileAndNeedDowload: Description
		/// <summary>Description</summary>
		private string _DowloadFileAndNeedDowload;
		/// <summary>Description</summary>
		public string DowloadFileAndNeedDowload { get => _DowloadFileAndNeedDowload; set => Set(ref _DowloadFileAndNeedDowload, value); }
		#endregion

		#region BlockSize: Description
		/// <summary>Description</summary>
		private int _BlockSize = 0;
		/// <summary>Description</summary>
		public int BlockSize
		{
			get => _BlockSize; set
			{
				Set(ref _BlockSize, value);
				if (_BlockSize < 2048)
					_BlockSize = 2048;
				if (_BlockSize > ServerBlockSize)
					_BlockSize = ServerBlockSize;
			}
		}
		#endregion
		#region Commands

		#region ChangePathCommand: Description
		private ICommand _ChangePathCommand;
		public ICommand ChangePathCommand => _ChangePathCommand ??= new LambdaCommand(OnChangePathCommandExecuted, CanChangePathCommandExecute);
		private bool CanChangePathCommandExecute(object e) => true;
		private void OnChangePathCommandExecuted(object e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			dialog.IsFolderPicker = true;
			if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileNames.Count() > 0)
			{
				if (Directory.Exists($"{dialog.FileName}\\{DowloadNode.Name}") || File.Exists($"{dialog.FileName}\\{DowloadNode.Name}"))
				{
					PathToDowload = $"{dialog.FileName}\\{DowloadNode.Name} ({System.IO.Path.GetRandomFileName()})";
					return;
				}
				PathToDowload = $"{dialog.FileName}\\{DowloadNode.Name}";
			}
		}
		#endregion
		#region StartDowloadCommand: Description
		private ICommand _StartDowloadCommand;
		public ICommand StartDowloadCommand => _StartDowloadCommand ??= new LambdaCommand(OnStartDowloadCommandExecuted, CanStartDowloadCommandExecute);
		private bool CanStartDowloadCommandExecute(object e) => true;
		private void OnStartDowloadCommandExecuted(object e)
		{
			SetModeWindow(ModeWindow.DowloadWindow);
			GenerateFolders();
			Task.Run(() => { StartDowload(); });
			Task.Run(() => { WatcherStatInfo(); });
		}
		#endregion
		#endregion

		#region Functions
		private void WatcherStatInfo()
		{
			while (!DowloadInfo.IsDowload)
			{
				try
				{
					var stat = DowloadInfo.StatsDowloads.Last();

					MaximumSize = stat.TotalSize;
					NowValue = stat.DowloadSize;
					NowPath = stat.Path;
					DowloadFileAndNeedDowload = stat.CountFilesDowloadAndNeedDowload;
					TextSpeed = $"{Utilities.RoundByte(stat.SpeedDowload)} / сек. ({Utilities.RoundByte(stat.DowloadSize)} \\ {Utilities.RoundByte(stat.TotalSize)})";
					Thread.Sleep(50);
				}
				catch (Exception ex) { Log.WriteLine(ex, LogLevel.Error); }

			}
		}
		private void ResumeDowload(string path)
		{
			if (!File.Exists(path))
			{
				MessageBoxHelper.ExclamationShow("Не получиться возобновить загрузку! Файла с данными не существует!");
				ClientProvider.SetResumePathDowload("");
				App.Current.Dispatcher.Invoke(() => { Window.Close(); });
				return;
			}
			SetModeWindow(ModeWindow.DowloadWindow);
			DowloadInfo = new DowloadInfo();
			DowloadInfo.PathSaveProgress = path;
			DowloadInfo.LoadInfo();
			Task.Run(() => { Dowload(); });
			Task.Run(() => { WatcherStatInfo(); });
			
		}
		private void Dowload()
		{
			StopwatchTimeDowload.Start();
			ClientProvider.DowloadFile(DowloadInfo);
			StopwatchTimeDowload.Stop();
			App.Current.Dispatcher.Invoke(() => { Window.Close(); });
			if (DowloadInfo.IsDowload)
			{
				MessageBoxHelper.InfoShow($"Скачивание завершенно за {StopwatchTimeDowload.Elapsed}!");
				File.Delete(Settings.Instance.Parametrs.LastDowload);
				ClientProvider.SetResumePathDowload("");
			}
		}
		private void StartDowload()
		{
			
			DowloadInfo.Files = DowloadItems;
			DowloadInfo.Abort = false;
			Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\dowloads");
			string path = $"{Directory.GetCurrentDirectory()}\\dowloads\\{System.IO.Path.GetRandomFileName()}.sharing.tmp";
			File.Create(path).Close();
			ClientProvider.SetResumePathDowload(path);
			DowloadInfo.PathSaveProgress = path;
			foreach (var i in DowloadInfo.Files)
			{
				i.RootPath = PathToDowload;
			}
			Dowload();
		}
		private void GenerateFolders()
		{
			DowloadItems.Clear();
			Directory.CreateDirectory(PathToDowload);
			void recursive(TreeViewItemFile itemFile)
			{
				foreach (var i in itemFile.Items)
				{
					if (i.IsChecked)
					{

						if (i.Item.IsFolder)
						{
							Directory.CreateDirectory($"{PathToDowload}{i.Item.Path}");
							recursive(i);
						}
						else
						{
							DowloadItems.Add(new RequestFileInfo() { Path = i.Item.Path, UID_ROOT = i.Item.UID_ROOT, TotalSize = i.Item.Size, Name = i.Item.Name, BlockSize = BlockSize });
						}
					}
				}
			}
			foreach (var i in Nodes)
			{
				if (i.IsChecked)
				{
					if (i.Item.IsFolder)
						recursive(i);
					else
					{
						DowloadItems.Add(new RequestFileInfo() { Path = i.Item.Path, UID_ROOT = i.Item.UID_ROOT, TotalSize = i.Item.Size, Name = i.Item.Name, BlockSize = BlockSize });
					}
				}
			}
		}
		public void GenereateTree()
		{
			Nodes.Clear();
			void Recursive(ItemTree item, TreeViewItemFile parent)
			{
				if (item.IsFolder)
				{
					var new_parent = new TreeViewItemFile();
					new_parent.Item = item;
					new_parent.Parent = parent;
					parent.Items.Add(new_parent);
					foreach (var i in item.ItemsTrees)
					{
						Recursive(i, new_parent);
					}
				}
				else
				{
					parent.Items.Add(new TreeViewItemFile() { Item = item, Parent = parent, TextSize = Utilities.RoundByte(item.Size) });
					parent.AddSizeByte(item.Size);
				}
			}
			if (DowloadNode.IsFolder)
			{
				var parent = new TreeViewItemFile();
				parent.Item = DowloadNode;
				Nodes.Add(parent);
				foreach (var i in DowloadNode.ItemsTrees)
				{
					Recursive(i, parent);
				}
			}
			else
			{
				Nodes.Add(new Models.TreeViewItemFile() { Item = DowloadNode, TextSize = Utilities.RoundByte(DowloadNode.Size) });
			}
		}
		private void SetModeWindow(ModeWindow mode)
		{
			VisibilityDowloadWindow = mode == ModeWindow.DowloadWindow ? Visibility.Visible : Visibility.Collapsed;
			VisibilitySelectFile = mode == ModeWindow.SelectFileDowload ? Visibility.Visible : Visibility.Collapsed;

			if (VisibilityDowloadWindow == Visibility.Visible)
			{
				Window.Width = 450;
				Window.Height = 150;
			}
		}
		public void Start(ModeWindow mode, bool Resume)
		{
			try
			{
				Window.Closed += Window_Closed;
				var set = ClientProvider.GetSettingsServer();
				ServerBlockSize = set.MaxSizeBlock;
				BlockSize = set.MaxSizeBlock;
				if (Resume)
				{
					ResumeDowload(Settings.Instance.Parametrs.LastDowload);
					return;
				}
				SetModeWindow(mode);
				PathToDowload = $"{KnownFolders.Downloads.Path}\\{DowloadNode.Name}";
				if (File.Exists(PathToDowload))
					PathToDowload += $" ({System.IO.Path.GetRandomFileName()})";
				GenereateTree();
				
			}
			catch (Exception ex)
			{
				Log.WriteLine(ex, LogLevel.Error);

				App.Current.Dispatcher.Invoke(() => { Window.Close(); });
				MessageBoxHelper.ErrorShow($"Произошла ошибка при открытии окна!\n{ex.Message}");
			}
		}

		private void Window_Closed(object? sender, EventArgs e)
		{
			DowloadInfo.Abort = true;
		}
		#endregion
	}
}
