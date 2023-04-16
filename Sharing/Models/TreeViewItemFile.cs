using Sharing.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharing.Models
{
	internal class TreeViewItemFile : Base.ViewModel.BaseViewModel
	{

		#region Parent: Description
		/// <summary>Description</summary>
		private TreeViewItemFile _Parent = null;
		/// <summary>Description</summary>
		public TreeViewItemFile Parent { get => _Parent; set => Set(ref _Parent, value); }
		#endregion
		#region Item: Description
		/// <summary>Description</summary>
		private ItemTree _Item;
		/// <summary>Description</summary>
		public ItemTree Item { get => _Item; set => Set(ref _Item, value); }
		#endregion


		#region SizeByte: Description
		/// <summary>Description</summary>
		private long _SizeByte = 0;
		/// <summary>Description</summary>
		public long SizeByte { get => _SizeByte; set => Set(ref _SizeByte, value); }
		#endregion

		#region TextSize: Description
		/// <summary>Description</summary>
		private string _TextSize = "0 Байт";
		/// <summary>Description</summary>
		public string TextSize { get => _TextSize; set => Set(ref _TextSize, value); }
		#endregion

		#region IsChecked: Description
		/// <summary>Description</summary>
		private bool _IsChecked = true;
		/// <summary>Description</summary>
		public bool IsChecked
		{
			get => _IsChecked; set
			{
				Set(ref _IsChecked, value);

				SetAllItemIsCkecked();
				if (value)
					SetParentChecked();
			}
		}
		#endregion

		#region Items: Description
		/// <summary>Description</summary>
		private ObservableCollection<TreeViewItemFile> _Items = new ObservableCollection<TreeViewItemFile>();
		/// <summary>Description</summary>
		public ObservableCollection<TreeViewItemFile> Items { get => _Items; set => Set(ref _Items, value); }
		#endregion

		public void SetAllItemIsCkecked()
		{
			foreach (var i in Items)
				i.IsChecked = _IsChecked;
		}
		public void SetParentChecked()
		{
			_IsChecked = true;
			OnPropertyChanged(nameof(IsChecked));
			if (Parent == null)
				return;
			Parent.SetParentChecked();
		}

		public void AddSizeByte(long size)
		{
			SizeByte += size;
			if (Parent != null)
				Parent.AddSizeByte(size);
			TextSize = Utilities.RoundByte(SizeByte);
		}
	}
}
