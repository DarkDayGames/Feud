using Feud.Server.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.Modal;
using Feud.Server.Services;

namespace Feud.Server
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRazorPages();
			services.AddServerSideBlazor();
			services.AddBlazoredModal();
			services.AddBlazoredLocalStorage();
			services.AddSingleton<IFeudHostService, FeudHostService>();
			services.AddSingleton<IFeudGameService, FeudGameService>();
			services.AddSingleton<IBoardEditingService, BoardEditingService>();
			services.AddSingleton<IBoardRepositoryService, BoardRepositoryService>();
			services.AddTransient<IDateTimeService, DateTimeService>();
			services.AddTransient<IBoardValidationService, BoardValidationService>();

			services.AddScoped<ICurrentPlayerNameService, CurrentPlayerNameService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				Program.Logger.Log(NLog.LogLevel.Info, "ENVIRONMENT: DEVELOPMENT");

				app.UseDeveloperExceptionPage();
			}
			else
			{
				Program.Logger.Log(NLog.LogLevel.Info, "ENVIRONMENT: PRODUCTION");

				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
			});
		}
	}
}
