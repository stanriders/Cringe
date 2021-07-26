using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Cringe.Bancho
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("Default", LogEventLevel.Debug)
                .WriteTo.Console()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Month)
                .CreateBootstrapLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) =>
                    configuration.ReadFrom.Configuration(context.Configuration)
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                        .MinimumLevel.Debug()
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext()
                        .WriteTo.Sentry(o => o.Dsn = services.GetService<IConfiguration>()?["SentryKey"])
                        .WriteTo.Console()
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
