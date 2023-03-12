using Microsoft.Extensions.Hosting;
using Sharing.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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


			LoadSettings();

			app.Run();
			
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
		}
		private static void SaveSettings()
		{
			XMLProvider.Save<Models.Parametrs>($"{Directory.GetCurrentDirectory()}\\Settings.xml", Settings.Instance);
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
