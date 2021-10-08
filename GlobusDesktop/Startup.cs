using System;
using System.Threading.Tasks;
using ElectronNET.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GlobusDesktop.Services;
using OpenTelemetry.Trace;

namespace GlobusDesktop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var jaegerAgentHost = Configuration["JaegerHost"];
            var jaegerAgentPort = Configuration["JaegerPort"];
            
            services.AddOpenTelemetryTracing(
                (builder) => builder
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
                    .AddJaegerExporter((options) =>
                    {
                        options.AgentHost = jaegerAgentHost;
                        options.AgentPort = Convert.ToInt32(jaegerAgentPort);
                    })
            );
            
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IGrafanaService, GrafanaService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
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

            Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
        }
    }
}