using System;
using Destructurama;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Cringe.Bancho
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                    .MinimumLevel.Override("Default", LogEventLevel.Debug)
                    .MinimumLevel.Override("System.Net.Http", LogEventLevel.Warning)
                    .Destructure.UsingAttributes()
                    .Enrich.WithProperty("Application", "Cringe.Bancho")
                    .WriteTo.Console()
                    .WriteTo.File("./logs/log_bancho.txt", rollingInterval: RollingInterval.Month)
                    .WriteTo.Seq("http://seq:5341")
                    .Enrich.FromLogContext()
                    .CreateBootstrapLogger();

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal("Howizi????? {Ex}", ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) =>
                    configuration.ReadFrom.Configuration(context.Configuration)
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                        .MinimumLevel.Override("System.Net.Http", LogEventLevel.Warning)
                        .MinimumLevel.Debug()
                        .Destructure.UsingAttributes()
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Application", "Cringe.Bancho")
                        .Enrich.WithClientIp()
                        .Enrich.WithRequestHeader("CF-IPCountry")
                        .Enrich.WithRequestHeader("Referer")
                        .Enrich.WithRequestHeader("User-Agent")
                        .WriteTo.Console()
                        .WriteTo.File("./logs/log_bancho.txt", rollingInterval: RollingInterval.Month)
                        .WriteTo.Seq("http://seq:5341"))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(o =>
                    {
                        o.ConfigureHttpsDefaults(o =>
                            o.ClientCertificateMode = ClientCertificateMode.NoCertificate);
                    });
                });
        }
    }
}
