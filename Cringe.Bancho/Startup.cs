using Cringe.Bancho.Services;
using Cringe.Database;
using Cringe.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using UAParser;

namespace Cringe.Bancho
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddDbContext<PlayerDatabaseContext>();
            services.AddDbContext<ScoreDatabaseContext>();
            services.AddDbContext<BeatmapDatabaseContext>();

            services.AddSingleton<LobbyService>();
            services.AddSingleton<SpectateService>();

            services.AddTransient<PlayersPool>();
            services.AddTransient<ChatService>();
            services.AddTransient<InvokeService>();
            services.AddTransient<TokenService>();
            services.AddTransient<PlayerTopscoreStatsCache>();
            services.AddTransient<PlayerRankRetriever>();
            services.AddTransient<FriendsService>();
            services.AddTransient<StatsService>();

            services.AddHostedService<AutoDisconnectService>();
            services.AddMemoryCache();

            services.AddControllers();
            services.AddSwaggerGen();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (context, httpContext) =>
                {
                    context.Set("UserId", httpContext.User.Identity?.Name);
                    var parsedUserAgent = Parser.GetDefault()?.Parse(httpContext.Request.Headers.UserAgent.ToString() ?? string.Empty);
                    context.Set("Browser", parsedUserAgent?.UA.ToString());
                    context.Set("Device", parsedUserAgent?.Device.ToString());
                    context.Set("OS", parsedUserAgent?.OS.ToString());
                };
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "osu!Cringe API");
            });

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
