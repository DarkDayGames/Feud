using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Feud.Server
{
	public class Program
	{

		public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		public static void Main(string[] args)
		{
			Logger.Log(NLog.LogLevel.Info, "--------------------------------------------");
			Logger.Log(NLog.LogLevel.Info, "Running Feud.Server...");

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
