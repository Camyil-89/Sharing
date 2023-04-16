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
using Sharing.API.Models;

namespace Sharing.API.Net
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
		public DowlaodFileInfo DowloadFile(RequestFileInfo requestFile)
		{
			var rqst = new HttpRequestMessage(HttpMethod.Get, $"{Url}/api/dowload/files");
			rqst.Content = JsonContent.Create(requestFile);

			var response = HttpClient.SendAsync(rqst).Result;
			return response.Content.ReadFromJsonAsync<DowlaodFileInfo>().Result;
		}

		public SettingsServer GetSettingsServer()
		{
			return Send("/api/settings").Content.ReadFromJsonAsync<SettingsServer>().Result;
		}
	}
}
