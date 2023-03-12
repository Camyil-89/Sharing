using System.Net.Sockets;

namespace Sharing.Server
{
	public static class Server
	{
		static TcpListener Listener;

		public static bool Started { get; private set; } = false;
		public static void Start(int port)
		{
			Listener = new TcpListener(port);
			Started = true;
			Task.Run(() =>
			{
				while (Started)
				{
					try
					{
						AcceptClient(Listener.AcceptTcpClient());			
					}
					catch (Exception ex)
					{

					}

				}
			});
		}

		private static void AcceptClient(TcpClient client)
		{

		}

		public static void Stop()
		{
			Listener.Stop();
			Listener = null;
			Started = false;
		}
	}
}