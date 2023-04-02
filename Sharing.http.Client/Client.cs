using System.Net;

namespace Sharing.Http.Client
{
	public enum Status : byte
	{
		OK = 1,
		Shutdown = 0,
	}

	public class Client
	{
		public Requests Requests { get; private set; }
		public string Url { get; private set; }
		public Status Status { get; private set; }
		public bool Connect(string url)
		{
			Status = Status.Shutdown;
			Url = url;
			Requests = new Requests(url);
			try
			{
				Requests.Ping();
				Status = Status.OK;
				return true;
			}
			catch { Status = Status.Shutdown; }
			return false;
		}
		public void Stop()
		{
			Status = Status.Shutdown;
			Requests = null;
		}
	}
}