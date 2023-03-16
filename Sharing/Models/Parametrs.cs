using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sharing.Models
{
	public class Parametrs : Base.ViewModel.BaseViewModel
	{

		#region ServerPort: Description
		/// <summary>Description</summary>
		private int _ServerPort = 3000;
		/// <summary>Description</summary>
		public int ServerPort
		{
			get => _ServerPort; set
			{
				if (value > 65535)
					value = 65535;
				else if (value < 3000)
					value = 3000;
				Set(ref _ServerPort, value);
			}
		}
		#endregion


		#region NetFindServerPort: Description
		/// <summary>Description</summary>
		private int _NetFindServerPort = 3010;
		/// <summary>Description</summary>
		public int NetFindServerPort { get => _NetFindServerPort; set => Set(ref _NetFindServerPort, value); }
		#endregion

		#region ClientNetFindServerPort: Description
		/// <summary>Description</summary>
		private int _ClientNetFindServerPort = 3010;
		/// <summary>Description</summary>
		public int ClientNetFindServerPort { get => _ClientNetFindServerPort; set => Set(ref _ClientNetFindServerPort, value); }
		#endregion


		#region ConnectAddressServer: Description
		/// <summary>Description</summary>
		private byte[] _ConnectAddressServer = new byte[] { 127, 0, 0, 1 };
		/// <summary>Description</summary>
		public byte[] ConnectAddressServer { get => _ConnectAddressServer; set => Set(ref _ConnectAddressServer, value); }
		#endregion


		#region ConnectPortServer: Description
		/// <summary>Description</summary>
		private int _ConnectPortServer = 3000;
		/// <summary>Description</summary>
		public int ConnectPortServer
		{
			get => _ConnectPortServer; set
			{
				if (value > 65535)
					value = 65535;
				else if (value < 3000)
					value = 3000;
				Set(ref _ConnectPortServer, value);

			}
		}
		#endregion


		#region SharingFilesAndFolders: Description
		/// <summary>Description</summary>
		private ObservableCollection<string> _SharingFilesAndFolders = new ObservableCollection<string>();
		/// <summary>Description</summary>
		public ObservableCollection<string> SharingFilesAndFolders { get => _SharingFilesAndFolders; set => Set(ref _SharingFilesAndFolders, value); }
		#endregion
	}
}
