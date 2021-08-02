using System;
using Destructurama;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Datadog;

namespace Cringe.Bancho
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                    .MinimumLevel.Override("Default", LogEventLevel.Debug)
                    .Destructure.UsingAttributes()
                    .WriteTo.Console()
                    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Month)
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
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                        .MinimumLevel.Debug()
                        .Destructure.UsingAttributes()
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext()
                        .WriteTo.Sentry(o => o.Dsn = services.GetService<IConfiguration>()?["SentryKey"])
                        .WriteTo.Console()
                        .WriteTo.Datadog(new DatadogConfiguration("127.0.0.1", 8125, "main", new[] {"cringe.bancho"}))
                        .WriteTo.File("log.txt", rollingInterval: RollingInterval.Month))
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
