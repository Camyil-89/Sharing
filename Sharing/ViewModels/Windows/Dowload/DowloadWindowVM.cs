using Sharing.API;
using Sharing.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharing.ViewModels.Windows.Dowload
{

	class DowloadWindowVM : Base.ViewModel.BaseViewModel
	{
		public DowloadWindowVM()
		{
			#region Commands
			#endregion
		}

		#region Parametrs

		#region DowloadNode: Description
		/// <summary>Description</summary>
		private ItemTree _DowloadNode;
		/// <summary>Description</summary>
		public ItemTree DowloadNode { get => _DowloadNode; set => Set(ref _DowloadNode, value); }
		#endregion
		#endregion

		#region Commands
		#endregion

		#region Functions
		public void Start()
		{
			Console.WriteLine($"{DowloadNode.Path}|{DowloadNode.Name}|{DowloadNode.UID_ROOT}|{DowloadNode.Size}");
		}
		#endregion
	}
}
