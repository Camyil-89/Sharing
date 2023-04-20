using Sharing.API;
using Sharing.API.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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


		#region ServerIPaddress: Description
		/// <summary>Description</summary>
		private byte[] _ServerIPaddress = new byte[] { 127, 0, 0, 1 };
		/// <summary>Description</summary>
		public byte[] ServerIPaddress { get => _ServerIPaddress; set => Set(ref _ServerIPaddress, value); }
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



		#region TextMaxSizeBlock: Description
		/// <summary>Description</summary>
		[XmlIgnore]
		private string _TextMaxSizeBlock = "";
		/// <summary>Description</summary>
		[XmlIgnore]
		public string TextMaxSizeBlock { get => _TextMaxSizeBlock; set => Set(ref _TextMaxSizeBlock, value); }
		#endregion


		#region MaxSizeBlock: Description
		/// <summary>Description</summary>
		private int _MaxSizeBlock = 1048576; // 1 mb
		/// <summary>Description</summary>
		public int MaxSizeBlock
		{
			get => _MaxSizeBlock; set
			{
				if (value < 2048)
					value = 2048;
				Set(ref _MaxSizeBlock, value);
				TextMaxSizeBlock = Utilities.RoundByte(value);
			}
		}
		#endregion

		#region SharingFilesAndFolders: Description	
		/// <summary>Description</summary>
		private ObservableCollection<API.Models.SharingFile> _SharingFilesAndFolders = new ObservableCollection<API.Models.SharingFile>();
		/// <summary>Description</summary>
		public ObservableCollection<API.Models.SharingFile> SharingFilesAndFolders { get => _SharingFilesAndFolders; set => Set(ref _SharingFilesAndFolders, value); }
		#endregion


		#region LastDowload: Description
		/// <summary>Description</summary>
		private string _LastDowload;
		/// <summary>Description</summary>
		public string LastDowload { get => _LastDowload; set => Set(ref _LastDowload, value); }
		#endregion
	}
}
