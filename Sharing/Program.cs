using Microsoft.Extensions.Hosting;
using Sharing.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Sharing
{
	public static class Program
	{
		public static Stopwatch Stopwatch = new Stopwatch();
		[STAThread]
		public static void Main(string[] args)
		{
			Directory.SetCurrentDirectory($"{new FileInfo(Process.GetCurrentProcess().MainModule.FileName).DirectoryName}");
			var app = new App();
			app.InitializeComponent();
			app.Exit += App_Exit;
			app.DispatcherUnhandledException += App_DispatcherUnhandledException;


			LoadSettings();
			app.Run();
			Services.Net.Server.ServerProvider.Stop();
		}

		private static void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBoxHelper.ErrorShow(e.Exception.ToString());
			Log.WriteError(e.Exception);
		}

		private static void App_Exit(object sender, ExitEventArgs e)
		{
			SaveSettings();
		}

		private static void LoadSettings()
		{
			try
			{
				Settings.Instance.Parametrs = XMLProvider.Load<Models.Parametrs>($"{Directory.GetCurrentDirectory()}\\Settings.xml");
			}
			catch { }

			Sharing.Services.Net.Server.ServerProvider.Init();
			Sharing.Services.Net.Client.ClientProvider.Init();
		}
		public static void SaveSettings()
		{
			XMLProvider.Save<Models.Parametrs>($"{Directory.GetCurrentDirectory()}\\Settings.xml", Settings.Instance.Parametrs);
		}
		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			var builder = Host.CreateDefaultBuilder(args);

			builder.UseContentRoot(Environment.CurrentDirectory);

			builder.ConfigureServices(App.ConfigureServices);


			return builder;
		}
	}
}
