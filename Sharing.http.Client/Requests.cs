using Sharing.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sharing.Http.Client
{
	public class Requests
	{
		private HttpClient HttpClient = new HttpClient();
		private string Url;
		public Requests(string url)
		{
			Url = url;
		}
		public DateTime LastUpdateTree()
		{
			return Send("/ping").Content.ReadFromJsonAsync<DateTime>().Result;
		}
		public double Ping()
		{
			DateTime time= DateTime.Now;
			Send("/ping");
			return (DateTime.Now - time).TotalMilliseconds;
		}
		public List<ItemTree> GetTree()
		{
			return Send("/api/sharing/files").Content.ReadFromJsonAsync<List<ItemTree>>().Result;
		}

		public HttpResponseMessage Send(string path)
		{
			return HttpClient.Send(new HttpRequestMessage(HttpMethod.Get, $"{Url}{path}"));
		}

		public HttpResponseMessage Send(string path, HttpRequestMessage message)
		{
			return HttpClient.Send(message);
		}
		public HttpResponseMessage SendPost<T>(string path, T obj)
		{
			HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, $"{Url}{path}");
			message.Content = JsonContent.Create(obj);
			return HttpClient.Send(message);
		}

		public void test()
		{
			//var request = new HttpRequestMessage(HttpMethod.Post, $"{Url}/api/dowload/files");
			var response = HttpClient.GetAsync($"{Url}/api/dowload/files").Result;
			var stream = response.Content.ReadAsStreamAsync().Result;

			byte[] buffer = new byte[4096];
			int bytesRead;
			while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
			{
				Console.WriteLine($"{stream.Length}>{Encoding.UTF8.GetString(buffer)}|{bytesRead}");
			}
		}
	}
}
