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
			app.Startup += App_Startup;
			app.DispatcherUnhandledException += App_DispatcherUnhandledException;


			LoadSettings();
			app.Run();
			Services.Net.Server.ServerProvider.Stop();
		}

		private static void App_Startup(object sender, StartupEventArgs e)
		{
			Task.Run(() =>
			{
				try
				{
					UpdaterAPI.GitHub.Downloader downloader = new UpdaterAPI.GitHub.Downloader();
					downloader.SetRootPath(Directory.GetCurrentDirectory());
					downloader.SetUrlUpdateInfo("Camyil-89/Sharing-Publish/main/UpdateInfo.xml");
					downloader.SetUrlDowloadRoot("Camyil-89/Sharing-Publish/main/versions");
					var tmp_path = $"{Directory.GetCurrentDirectory()}\\update";
					var last_version = downloader.GetLastVerison(UpdaterAPI.Models.TypeVersion.Release);

					if (last_version.Version != Settings.Instance.Version && MessageBoxHelper.QuestionShow($"Доступна новая версия {last_version.Version}\nСкачать?") == MessageBoxResult.Yes)
					{
						App.Current.Dispatcher.Invoke(() =>
						{
							var status = UpdateAPIUI.UIProvider.DowloadFilesWithInstaller(downloader, last_version, tmp_path);
							if (status == UpdateAPIUI.Models.StatusDowload.OK)
							{
								downloader.CopyFilesFromTempDirectory(tmp_path, $"taskkill /pid {Process.GetCurrentProcess().Id} &&", $"&& rmdir /s /q \"{Directory.GetCurrentDirectory()}\\update\" && \"{Process.GetCurrentProcess().MainModule.FileName.Split("\\").Last()}\"");
							}
							else
							{
								Directory.Delete(tmp_path, true);
							}
						});
					}
				}
				catch (Exception ex) { Console.WriteLine(ex); }
			});
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
			if (!File.Exists(Settings.Instance.Parametrs.LastDowload) && Directory.Exists($"{Directory.GetCurrentDirectory()}\\dowloads"))
			{
				foreach (var i in Directory.GetFiles($"{Directory.GetCurrentDirectory()}\\dowloads"))
					File.Delete(i);
			}
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
