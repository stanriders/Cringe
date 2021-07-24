
using Cringe.Bancho.Services;
using Cringe.Database;
using Cringe.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddDbContext<PlayerDatabaseContext>();
            services.AddDbContext<ScoreDatabaseContext>();
            services.AddDbContext<BeatmapDatabaseContext>();
            services.AddSingleton<LobbyService>();
            services.AddTransient<PlayersPool>();
            services.AddTransient<ChatService>();
            services.AddSingleton<StatsService>();
            services.AddTransient<InvokeService>();
            services.AddTransient<TokenService>();
            services.AddTransient<PlayerTopscoreStatsCache>();
            services.AddTransient<PlayerRankCache>();
            
            services.AddMemoryCache();

            services.AddControllers();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

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