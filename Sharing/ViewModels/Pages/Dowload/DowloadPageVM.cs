using Microsoft.Xaml.Behaviors;
using Sharing.API;
using Sharing.API.Models;
using Sharing.Base.Command;
using Sharing.Services;
using Sharing.ViewModels.Windows.Dowload;
using Sharing.Views.Windows.Dowload;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Sharing.ViewModels.Pages.Dowload
{
	public class BindableSelectedItemBehavior : Behavior<TreeView>
	{
		#region SelectedItem Property


		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(BindableSelectedItemBehavior), new UIPropertyMetadata(null, OnSelectedItemChanged));
		public object SelectedItem
		{
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}


		private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var item = e.NewValue as TreeViewItem;
			if (item != null)
			{
				item.SetValue(TreeViewItem.IsSelectedProperty, true);
			}
		}

		#endregion

		protected override void OnAttached()
		{
			base.OnAttached();
			this.AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			if (this.AssociatedObject != null)
			{
				this.AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
			}
		}

		private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			this.SelectedItem = e.NewValue;
		}
	}
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


		#region SelectedNode: Description
		/// <summary>Description</summary>
		private TreeViewItem _SelectedNode;
		/// <summary>Description</summary>
		public TreeViewItem SelectedNode { get => _SelectedNode; set => Set(ref _SelectedNode, value); }
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
			if (Services.Net.Client.ClientProvider.GetStatus() == Http.Client.Status.Shutdown)
				Services.Net.Client.ClientProvider.Start(new System.Net.IPAddress(new byte[] { 192, 168, 1, 65 }), Settings.Instance.Parametrs.ConnectPortServer);
			else
				Services.Net.Client.ClientProvider.Stop();
		}
		#endregion

		#region DowloadNodeCommand: Description
		private ICommand _DowloadNodeCommand;
		public ICommand DowloadNodeCommand => _DowloadNodeCommand ??= new LambdaCommand(OnDowloadNodeCommandExecuted, CanDowloadNodeCommandExecute);
		private bool CanDowloadNodeCommandExecute(object e) => SelectedNode != null;
		private void OnDowloadNodeCommandExecuted(object e)
		{
			DowloadWindow window = new DowloadWindow();
			Console.WriteLine(SelectedNode);
			DowloadWindowVM vm = new DowloadWindowVM();
			vm.DowloadNode = SelectedNode.Tag as ItemTree;
			vm.Start();
			window.DataContext = vm;

			window.Owner = App.Current.MainWindow;
			window.Show();
		}
		#endregion
		#endregion

		#region Functions
		#endregion
	}
}
