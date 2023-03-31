using Sharing.Services;

namespace Sharing.Http.Server.Services
{
	public class Server
	{
		static WebApplication WebApplication;
		public static bool IsStarted { get; private set; } = false;
		public static void Start(string[] args)
		{
			Console.WriteLine(string.Join(";", args));
			var builder = WebApplication.CreateBuilder(args);
			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			WebApplication = builder.Build();

			// Configure the HTTP request pipeline.
			if (WebApplication.Environment.IsDevelopment())
			{
				WebApplication.UseSwagger();
				WebApplication.UseSwaggerUI();
			}

			WebApplication.UseHttpsRedirection();

			WebApplication.UseAuthorization();

			WebApplication.MapControllers();

			Task.Run(() =>
			{
				try
				{
					Log.WriteLine("HttpServer: start");
					IsStarted = true;
					WebApplication.Run();
					IsStarted = false;
					Log.WriteLine("HttpServer: stop");
				}
				catch (Exception ex) { IsStarted = false; Log.WriteLine(ex, Sharing.Services.LogLevel.Error); }
			});
		}
		public static void Stop()
		{
			WebApplication.StopAsync();
		}
	}
}
